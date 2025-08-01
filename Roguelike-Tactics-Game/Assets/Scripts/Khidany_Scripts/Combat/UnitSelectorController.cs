using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectorController : MonoBehaviour
{
    [Header("References")]
    public CameraPointer pointer;                       // Controller pointer
    public PathPreviewController pathPreview;          // Path preview arrows
    public PointerHighlighter pointerHighlighter;      // Grid highlight visual
    public GridInitializer gridInitializer;            // For GridMap
    public InputActionAsset inputActions;              // Input System actions

    private GridMap grid;
    private PlayerUnit selectedUnit = null;
    private Vector2Int lastHoverTile = new Vector2Int(-999, -999);

    private enum State { Idle, UnitSelected }
    private State currentState = State.Idle;

    private enum InputMode { Mouse, Controller }
    private InputMode currentInputMode = InputMode.Mouse;

    // Input Actions
    private InputAction confirmAction;
    private InputAction cancelAction;
    private InputAction nextUnitAction;
    private InputAction prevUnitAction;

    void Start()
    {
        // Get grid reference
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
        if (confirmAction != null) confirmAction.performed -= OnConfirm;
        if (cancelAction != null) cancelAction.performed -= OnCancel;
        if (nextUnitAction != null) nextUnitAction.performed -= OnNextUnit;
        if (prevUnitAction != null) prevUnitAction.performed -= OnPrevUnit;
    }

    void Update()
    {
        if (grid == null) return;

        // Detect input mode
        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            currentInputMode = InputMode.Mouse;
        else if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue() != Vector2.zero)
            currentInputMode = InputMode.Controller;

        // Determine hover world position based on mode
        Vector3 worldPos;
        if (currentInputMode == InputMode.Mouse)
        {
            worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPos.z = 0;
        }
        else
        {
            worldPos = pointer.transform.position;
        }

        // Update hover and highlights
        UpdateHoverTile(worldPos);
        if (pointerHighlighter != null)
            pointerHighlighter.UpdateHighlight(worldPos);

        // Keyboard fallback for Cancel (Escape) and Confirm (Left Click)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                HandleCancel();
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleConfirm(worldPos);
        }
    }

    private void UpdateHoverTile(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out int hx, out int hy);
        Vector2Int hoverTile = new Vector2Int(hx, hy);

        if (hoverTile != lastHoverTile)
        {
            lastHoverTile = hoverTile;

            if (currentState == State.UnitSelected)
            {
                if (IsTileHighlighted(hoverTile))
                    pathPreview.UpdatePathPreview(hoverTile);
                else
                    pathPreview.ClearPath();
            }
        }
    }

    private bool IsTileHighlighted(Vector2Int tile)
    {
        return selectedUnit != null && selectedUnit.HasMoveHighlight(tile);
    }

    // Input System confirm
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        Vector3 worldPos = GetActiveWorldPosition();
        HandleConfirm(worldPos);
    }

    // Input System cancel
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        HandleCancel();
    }

    private void HandleConfirm(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out int tx, out int ty);
        Vector2Int targetTile = new Vector2Int(tx, ty);

        if (currentState == State.Idle)
        {
            PlayerUnit unit = FindUnitAtTile(targetTile);
            if (unit != null && !unit.HasActed)
            {
                SelectUnit(unit);
                unit.Select();
            }
        }
        else if (currentState == State.UnitSelected)
        {
            if (IsTileHighlighted(targetTile))
            {
                var path = PlayerPathfinder.FindPath(grid, selectedUnit.GetGridPosition(), targetTile, selectedUnit.Movement);
                if (path != null)
                {
                    selectedUnit.GetComponent<UnitMover>().MoveAlong(path, () =>
                    {
                        Debug.Log("[UnitSelector] Movement finished callback.");
                        selectedUnit.MarkActed();
                        TurnManager.Instance.NotifyUnitActed();
                        DeselectUnit();  // Optional: deselect after movement finishes
                    });
                    pathPreview.ClearPath();
                    return;
                }

            }
            Debug.Log("[UnitSelector] Cannot move to that tile.");
        }
    }

    private void HandleCancel()
    {
        if (currentState == State.UnitSelected)
        {
            pathPreview.ClearPath();
            DeselectUnit();
        }
    }

    // Cycle units
    private void OnNextUnit(InputAction.CallbackContext ctx) => JumpToUnit(1);
    private void OnPrevUnit(InputAction.CallbackContext ctx) => JumpToUnit(-1);

    private void JumpToUnit(int direction)
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
        List<PlayerUnit> availableUnits = new List<PlayerUnit>();

        foreach (var u in units)
            if (!u.HasActed) availableUnits.Add(u);

        if (availableUnits.Count == 0)
        {
            Debug.Log("[UnitSelector] No available units to cycle to.");
            return;
        }

        availableUnits.Sort((a, b) =>
            (a.GetGridPosition().x + a.GetGridPosition().y).CompareTo(
            (b.GetGridPosition().x + b.GetGridPosition().y)));

        int px, py;
        grid.GetXY(pointer.transform.position, out px, out py);
        Vector2Int pointerTile = new Vector2Int(px, py);

        int currentIndex = -1;
        for (int i = 0; i < availableUnits.Count; i++)
            if (availableUnits[i].GetGridPosition() == pointerTile) currentIndex = i;

        int nextIndex = (currentIndex + direction + availableUnits.Count) % availableUnits.Count;

        Vector3 worldPos = grid.GetWorldPosition(
            availableUnits[nextIndex].GetGridPosition().x,
            availableUnits[nextIndex].GetGridPosition().y
        ) + Vector3.one * (grid.CellSize / 2f);

        pointer.transform.position = worldPos;
        if (currentState == State.UnitSelected)
            DeselectUnit();
    }

    private void SelectUnit(PlayerUnit unit)
    {
        selectedUnit = unit;
        currentState = State.UnitSelected;
        pathPreview.SetSelectedUnit(unit);
    }

    private void DeselectUnit()
    {
        if (selectedUnit != null)
            selectedUnit.Deselect();
        selectedUnit = null;
        currentState = State.Idle;
        lastHoverTile = new Vector2Int(-999, -999);
        pathPreview.ClearPath();
    }

    private PlayerUnit FindUnitAtTile(Vector2Int tile)
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
        foreach (var u in units)
            if (u.GetGridPosition() == tile) return u;
        return null;
    }

    private Vector3 GetActiveWorldPosition()
    {
        if (currentInputMode == InputMode.Mouse)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pos.z = 0;
            return pos;
        }
        else
        {
            return pointer.transform.position;
        }
    }
}
