using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerUnit))]            // Change to base Unit script later
public class UnitMover : MonoBehaviour
{
    // How long to hop from one tile center to the next.
    public float stepDuration = 0.15f;

    private PlayerUnit unit;
    private GridMap grid;

    private void Awake()
    {
        unit = GetComponent<PlayerUnit>();
    }

    public void Init(GridMap g) => grid = g;

    public void MoveAlong(List<Vector2Int> path)
    {
        if (path == null || path.Count < 2) return; // already on target
        StartCoroutine(MoveCo(path));
    }

    private IEnumerator MoveCo(List<Vector2Int> path)
    {
        unit.MarkActed();       
        GridManager.Instance.ClearSelectedUnit();

        // free start tile
        grid.MarkUnoccupied(unit.GetGridPosition());

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 from = grid.GetWorldPosition(
                path[i - 1].x, path[i - 1].y) + Vector3.one * (grid.CellSize / 2f);
            Vector3 to = grid.GetWorldPosition(
                path[i].x, path[i].y) + Vector3.one * (grid.CellSize / 2f);

            float t = 0f;
            while (t < stepDuration)
            {
                transform.position = Vector3.Lerp(from, to, t / stepDuration);
                t += Time.deltaTime;
                yield return null;
            }
            transform.position = to;
        }

        // occupy final tile
        unit.SetGridPos(path[^1]);   // add this setter to PlayerUnit
        grid.MarkOccupied(unit.GetGridPosition());

        TurnManager.Instance.NotifyUnitActed();
    }
}
