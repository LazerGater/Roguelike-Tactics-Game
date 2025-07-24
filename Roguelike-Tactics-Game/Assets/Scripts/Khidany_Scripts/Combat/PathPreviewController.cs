using System.Collections.Generic;
using UnityEngine;

public class PathPreviewController : MonoBehaviour
{
    [Header("References")]
    public GameObject arrowPrefab; // Arrow prefab for path steps

    private GridMap grid;
    private PlayerUnit selectedUnit;
    private List<GameObject> activeArrows = new List<GameObject>();

    void Start()
    {
        // Auto-find grid
        GridInitializer gridInit = FindFirstObjectByType<GridInitializer>();
        if (gridInit != null)
            grid = gridInit.Grid;

        if (arrowPrefab == null)
            Debug.LogError("[PathPreviewController] Arrow prefab not assigned!");
    }

    /// <summary>
    /// Called by UnitSelectorController when a unit is selected.
    /// </summary>
    public void SetSelectedUnit(PlayerUnit unit)
    {
        selectedUnit = unit;
        ClearPath();
    }

    /// <summary>
    /// Clears path when deselecting or confirming.
    /// </summary>
    public void ClearPath()
    {
        foreach (var arrow in activeArrows)
            if (arrow) Destroy(arrow);

        activeArrows.Clear();
    }

    /// <summary>
    /// Updates the path preview to the hovered position.
    /// </summary>
    public void UpdatePathPreview(Vector2Int hoverTile)
    {
        if (selectedUnit == null || grid == null)
        {
            ClearPath();
            return;
        }

        // Find path using PlayerPathfinder
        var path = PlayerPathfinder.FindPath(grid, selectedUnit.GetGridPosition(), hoverTile, selectedUnit.Stats.moveRange);

        // If no path or path length < 2 -> clear and return
        if (path == null || path.Count < 2)
        {
            ClearPath();
            return;
        }

        // Clear previous arrows
        ClearPath();

        // Draw arrows along the path (skip index 0, the unit's current position)
        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int from = path[i - 1];
            Vector2Int to = path[i];

            // Compute world positions
            Vector3 fromWorld = grid.GetWorldPosition(from.x, from.y) + Vector3.one * (grid.CellSize / 2f);
            Vector3 toWorld = grid.GetWorldPosition(to.x, to.y) + Vector3.one * (grid.CellSize / 2f);

            // Create arrow
            GameObject arrow = Instantiate(arrowPrefab, fromWorld, Quaternion.identity);

            // Rotate arrow to face the direction of the segment
            Vector3 dir = (toWorld - fromWorld).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 to align prefab pointing up

            activeArrows.Add(arrow);
        }
    }
}
