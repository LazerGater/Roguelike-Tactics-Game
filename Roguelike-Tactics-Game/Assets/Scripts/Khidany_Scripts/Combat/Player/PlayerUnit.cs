using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class PlayerUnit : MonoBehaviour, IBattleUnit
{
    [Header("Data References")]
    public CharacterData characterData;
    public ClassData classData;
    public UnitStats Stats { get; private set; }

    [SerializeField] private GameObject moveHighlightPrefab;

    private GridMap grid;
    private Vector2Int gridPos;
    private bool isSelected = false;

    private readonly List<GameObject> moveHighlights = new List<GameObject>();

    public bool HasActed { get; private set; } = false;

    private static readonly Vector2Int[] DIRS = {
        new Vector2Int( 1, 0), new Vector2Int(-1, 0),
        new Vector2Int( 0, 1), new Vector2Int( 0,-1)
    };

    public void Initialize(CharacterData cData, ClassData clData, GridMap g, Vector2Int startPos)
    {
        characterData = cData;
        classData = clData;
        Stats = new UnitStats(characterData, classData);

        grid = g;
        gridPos = startPos;

        transform.position = grid.GetWorldPosition(gridPos.x, gridPos.y)
                           + Vector3.one * (grid.CellSize / 2f);

        if (!grid.TryMarkOccupied(gridPos))
        {
            Debug.LogError($"Spawn clash at {gridPos}! Destroying self.");
            Destroy(gameObject);
        }

        GetComponent<UnitMover>().Init(grid);
    }

    public void SetGridPos(Vector2Int p) => gridPos = p;
    public Vector2Int GetGridPosition() => gridPos;

    public void MarkActed() => HasActed = true;
    public void ResetTurn() => HasActed = false;

    private void Update()
    {
        if (TurnManager.Instance == null || !TurnManager.Instance.IsBattleActive) return;
        if (HasActed) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = AshenUtil.GridItems.GetMouseWorldPosition();
            grid.GetXY(mouseWorld, out int cx, out int cy);
            Vector2Int clicked = new Vector2Int(cx, cy);

            if (clicked == gridPos)
            {
                GridManager.Instance.SelectUnit(this);
                return;
            }

            if (isSelected && IsHighlighted(clicked))
            {
                var path = PlayerPathfinder.FindPath(grid, gridPos, clicked, Stats.moveRange);
                if (path != null)
                    GetComponent<UnitMover>().MoveAlong(path);
            }
        }
    }

    public void Select()
    {
        isSelected = true;
        ShowMoveRange();
    }

    public void Deselect()
    {
        isSelected = false;
        ClearMoveRange();
    }

    private void ShowMoveRange()
    {
        ClearMoveRange();
        var costSoFar = new Dictionary<Vector2Int, int>();
        var frontier = new Queue<Vector2Int>();

        costSoFar[gridPos] = 0;
        frontier.Enqueue(gridPos);

        while (frontier.Count > 0)
        {
            Vector2Int cur = frontier.Dequeue();
            int c0 = costSoFar[cur];

            foreach (var dir in DIRS)
            {
                Vector2Int nxt = cur + dir;
                if (!grid.IsInBounds(nxt.x, nxt.y)) continue;
                if (grid.IsOccupied(nxt)) continue;

                int tileCost = grid.GetValue(nxt.x, nxt.y);
                if (tileCost <= 0) continue;

                int c1 = c0 + tileCost;
                if (c1 > Stats.moveRange) continue;

                if (costSoFar.TryGetValue(nxt, out int saved) && saved <= c1)
                    continue;

                costSoFar[nxt] = c1;
                frontier.Enqueue(nxt);
            }
        }

        foreach (var kvp in costSoFar)
        {
            if (kvp.Key == gridPos) continue;
            Vector3 wp = grid.GetWorldPosition(kvp.Key.x, kvp.Key.y)
                       + Vector3.one * (grid.CellSize / 2f);
            var h = Instantiate(moveHighlightPrefab, wp, Quaternion.identity);
            h.name = $"Highlight_{kvp.Key.x}_{kvp.Key.y}";
            moveHighlights.Add(h);

        }
    }

    private void ClearMoveRange()
    {
        foreach (var h in moveHighlights) if (h) Destroy(h);
        moveHighlights.Clear();
    }

    private bool IsHighlighted(Vector2Int pos)
    {
        return moveHighlights.Exists(obj =>
        {
            var s = obj.name.Split('_');
            return int.Parse(s[1]) == pos.x && int.Parse(s[2]) == pos.y;
        });
    }
    public int Movement => Stats != null ? Stats.moveRange : 0;

    public bool HasMoveHighlight(Vector2Int tile)
    {
        return moveHighlights.Exists(obj =>
        {
            // NOTE: MoveHighlight objects currently don't have their grid pos in name
            // Add this naming in ShowMoveRange() below if missing:
            // h.name = $"Highlight_{kvp.Key.x}_{kvp.Key.y}";
            var s = obj.name.Split('_');
            return s.Length >= 3 && int.Parse(s[1]) == tile.x && int.Parse(s[2]) == tile.y;
        });
    }
}
