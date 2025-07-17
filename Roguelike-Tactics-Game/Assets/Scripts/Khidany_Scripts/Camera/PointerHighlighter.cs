using UnityEngine;

public class PointerHighlighter : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                       // Drag your main camera if needed
    public GameObject highlightPrefab;       // Pointer highlight prefab

    [Header("Settings")]
    public GridMap grid;                     // Assign the GridMap reference

    private GameObject currentHighlight;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        GridInitializer gridInit = FindFirstObjectByType<GridInitializer>();
        if (gridInit != null)
            grid = gridInit.Grid;

        // Instantiate the highlight prefab and hide it
        if (highlightPrefab != null)
        {
            currentHighlight = Instantiate(highlightPrefab, Vector3.zero, Quaternion.identity);
            currentHighlight.SetActive(false);
        }
        else
        {
            Debug.LogError("[PointerHighlighter] Highlight prefab is not assigned!");
        }
    }

    void Update()
    {
        if (currentHighlight == null || grid == null) return;

        // Get world position of pointer (from CameraPointer)
        Vector3 pointerWorldPos = transform.position;

        // Determine which grid cell it's over
        grid.GetXY(pointerWorldPos, out int x, out int y);

        // Check if inside grid
        if (!grid.IsInBounds(x, y))
        {
            currentHighlight.SetActive(false);
            return;
        }

        // Snap highlight to that cell's center
        Vector3 cellCenter = grid.GetWorldPosition(x, y) + Vector3.one * (grid.CellSize / 2f);
        currentHighlight.transform.position = cellCenter;

        // Make sure it's visible
        if (!currentHighlight.activeSelf)
            currentHighlight.SetActive(true);
    }

    public void HideHighlight()
    {
        if (currentHighlight != null)
            currentHighlight.SetActive(false);
    }
}
