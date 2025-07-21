using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    private GridMap grid;
    private IBattleUnit unit;

    public void Init(GridMap gridMap)
    {
        grid = gridMap;
        unit = GetComponent<IBattleUnit>();
        if (unit == null)
        {
            Debug.LogError("UnitMover requires a component that implements IBattleUnit!");
        }
    }

    public void MoveAlong(List<Vector2Int> path)
    {
        if (unit == null || grid == null || path == null || path.Count == 0)
            return;

        StartCoroutine(MoveRoutine(path));
    }

    private IEnumerator MoveRoutine(List<Vector2Int> path)
    {
        foreach (var pos in path)
        {
            Vector3 targetWorld = grid.GetWorldPosition(pos.x, pos.y) + Vector3.one * (grid.CellSize / 2f);

            while (Vector3.Distance(transform.position, targetWorld) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorld, 5f * Time.deltaTime);
                yield return null;
            }

            unit.SetGridPos(pos);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
