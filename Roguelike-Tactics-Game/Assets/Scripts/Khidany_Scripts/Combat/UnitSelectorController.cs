using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectorController : MonoBehaviour
{
    [Header("References")]
    public CameraPointer pointer;                       // Pointer object for controller
    public PathPreviewController pathPreview;          // Handles path arrow previews
    public PointerHighlighter pointerHighlighter;      // Handles grid hover highlight
    public GridInitializer gridInitializer;            // Provides GridMap reference
    public InputActionAsset inputActions;              // Default Input Action asset

    private GridMap grid;
    private PlayerUnit selectedUnit = null;
    private Vector2Int lastHoverTile = new Vector2Int(-999, -999);

    // Selection state
    private enum State { Idle, UnitSelected }
    private State currentState = State.Idle;

    // Input Actions
    private InputAction confirmAction;
    private InputAction cancelAction;
    private InputAction nextUnitAction;
    private InputAction prevUnitAction;

    void Start()
    {
        // Get GridMap reference from GridInitializer
        if (gridInitializer == null)
            gridInitializer = FindFirstObjectByType<GridInitializer>();
        if (gridInitializer != null)
            grid = gridInitializer.Grid;

        // Bind input actions
        if (inputActions != null)
        {
            confirmAction = inputActions.FindAction("Confirm");
            cancelAction = inputActions.FindAction("Cancel");
            nextUnitAction = inputActions.FindAction("Next");
            prevUnitAction = inputActions.FindAction("Previous");

            if (confirmAction != null) confirmAction.performed += OnConfirm;
            if (cancelAction != null) cancelAction.performed += OnCancel;
            if (nextUnitAction != null) nextUnitAction.performed += OnNextUnit;
            if (prevUnitAction != null) prevUnitAction.performed += OnPrevUnit;

            if (confirmAction != null) confirmAction.Enable();
            if (cancelAction != null) cancelAction.Enable();
            if (nextUnitAction != null) nextUnitAction.Enable();
            if (prevUnitAction != null) prevUnitAction.Enable();
        }
    }

    void OnDestroy()
    {
        // Remove input listeners
        if (confirmAction != null) confirmAction.performed -= OnConfirm;
        if (cancelAction != null) cancelAction.performed -= OnCancel;
        if (nextUnitAction != null) nextUnitAction.performed -= OnNextUnit;
        if (prevUnitAction != null) prevUnitAction.performed -= OnPrevUnit;
    }

    void Update()
    {
        if (grid == null || pointer == null) return;

        // Controller hover (pointer position)
        UpdateHoverTile(pointer.transform.position);

        // Mouse hover override (if mouse is moved)
        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0;
            UpdateHoverTile(mouseWorld);
        }
    }

    // Handles updating the hover tile and path preview
    private void UpdateHoverTile(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out int hx, out int hy);
        Vector2Int hoverTile = new Vector2Int(hx, hy);

        if (hoverTile != lastHoverTile)
        {
            lastHoverTile = hoverTile;

            // Only update path preview when a unit is selected
            if (currentState == State.UnitSelected)
            {
                pathPreview.UpdatePathPreview(hoverTile);
            }
        }
    }

    private bool IsTileHighlighted(Vector2Int tile)
    {
        return selectedUnit != null && selectedUnit.HasMoveHighlight(tile);
    }

    // Confirm button logic (controller or keyboard)
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (currentState == State.Idle)
        {
            // Try selecting a unit
            grid.GetXY(pointer.transform.position, out int px, out int py);
            PlayerUnit unit = FindUnitAtTile(new Vector2Int(px, py));
            if (unit != null && !unit.HasActed)
            {
                SelectUnit(unit);
                unit.Select(); // Show its move range
            }
        }
        else if (currentState == State.UnitSelected)
        {
            // Attempt to move the selected unit
            grid.GetXY(pointer.transform.position, out int tx, out int ty);
            Vector2Int targetTile = new Vector2Int(tx, ty);

            Debug.Log($"Target tile: {targetTile}, Occupied: {grid.IsOccupied(targetTile)}");

            // Validate highlighted tile
            if (IsTileHighlighted(targetTile))
            {
                var path = PlayerPathfinder.FindPath(grid, selectedUnit.GetGridPosition(), targetTile, selectedUnit.maxMovePoints);
                if (path != null)
                {
                    selectedUnit.GetComponent<UnitMover>().MoveAlong(path);
                    pathPreview.ClearPath();
                    DeselectUnit();
                    return;
                }
            }

            Debug.Log("[UnitSelector] Cannot move to that tile.");
        }
    }

    // Cancel button logic
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentState == State.UnitSelected)
        {
            Debug.Log("[UnitSelector] Cancel move!");
            pathPreview.ClearPath();
            DeselectUnit();
        }
    }

    // Cycle to next available unit
    private void OnNextUnit(InputAction.CallbackContext ctx)
    {
        JumpToUnit(1);
    }

    // Cycle to previous available unit
    private void OnPrevUnit(InputAction.CallbackContext ctx)
    {
        JumpToUnit(-1);
    }

    private void JumpToUnit(int direction)
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
        List<PlayerUnit> availableUnits = new List<PlayerUnit>();

        foreach (var u in units)
        {
            if (!u.HasActed)
                availableUnits.Add(u);
        }

        if (availableUnits.Count == 0)
        {
            Debug.Log("[UnitSelector] No available units to cycle to.");
            return;
        }

        // Sort units for consistent order
        availableUnits.Sort((a, b) =>
            (a.GetGridPosition().x + a.GetGridPosition().y).CompareTo(
            (b.GetGridPosition().x + b.GetGridPosition().y)));

        // Find current pointer position
        grid.GetXY(pointer.transform.position, out int px, out int py);
        Vector2Int pointerTile = new Vector2Int(px, py);

        int currentIndex = -1;
        for (int i = 0; i < availableUnits.Count; i++)
        {
            if (availableUnits[i].GetGridPosition() == pointerTile)
            {
                currentIndex = i;
                break;
            }
        }

        // Compute next index
        int nextIndex = (currentIndex + direction + availableUnits.Count) % availableUnits.Count;

        // Move pointer to next unit
        Vector3 worldPos = grid.GetWorldPosition(
            availableUnits[nextIndex].GetGridPosition().x,
            availableUnits[nextIndex].GetGridPosition().y
        ) + Vector3.one * (grid.CellSize / 2f);

        pointer.transform.position = worldPos;
        Debug.Log($"[UnitSelector] Jumped to unit at {availableUnits[nextIndex].GetGridPosition()}");

        // Deselect any current selection
        if (currentState == State.UnitSelected)
            DeselectUnit();
    }

    private void SelectUnit(PlayerUnit unit)
    {
        selectedUnit = unit;
        currentState = State.UnitSelected;
        pathPreview.SetSelectedUnit(unit);
        Debug.Log($"[UnitSelector] Selected unit at {unit.GetGridPosition()}");
    }

    private void DeselectUnit()
    {
        if (selectedUnit != null)
            selectedUnit.Deselect(); // Clear move highlights on the unit

        selectedUnit = null;
        currentState = State.Idle;
        lastHoverTile = new Vector2Int(-999, -999);
        pathPreview.ClearPath();
    }

    private PlayerUnit FindUnitAtTile(Vector2Int tile)
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
        foreach (var u in units)
        {
            if (u.GetGridPosition() == tile)
                return u;
        }
        return null;
    }
}
