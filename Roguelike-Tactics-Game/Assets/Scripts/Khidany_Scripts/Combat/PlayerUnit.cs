using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class PlayerUnit : MonoBehaviour
{
    /* -------------------------------------------------
       1. Data 
    ----------------------------------------------- */
    private PlayerData dataRef;

    // core stats (will be overwritten by SetupFromData)
    public int maxHP = 20;
    public int atk = 5;
    public int def = 2;
    public int speed = 3;
    public int luck = 1;
    public int dex = 2;

    // mobility
    public int maxMovePoints = 4;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject moveHighlightPrefab;

    /* -------------------------------------------
       2. Grid related fields
       --------------------------------------------- */
    private GridMap grid;
    private Vector2Int gridPos;
    public void SetGridPos(Vector2Int p) => gridPos = p;   // used by UnitMover

    /* -----------------------------------------
       3. Selection / Move range helpers
       ------------------------------------------ */
    private bool isSelected = false;
    private readonly List<GameObject> moveHighlights = new List<GameObject>();

    private static readonly Vector2Int[] DIRS = {
        new Vector2Int( 1, 0), new Vector2Int(-1, 0),
        new Vector2Int( 0, 1), new Vector2Int( 0,-1)
    };

    /* -------------------------------------
       4. Turn state
       -------------------------------------- */
    public bool HasActed { get; private set; } = false;
    public void MarkActed() => HasActed = true;   // called by UnitMover
    public void ResetTurn() => HasActed = false;

    /* -------------------------------------
       5. Initialisation
       ------------------------------------ */
    public void Init(GridMap g, Vector2Int startPos)
    {
        grid = g;
        gridPos = startPos;

        transform.position = grid.GetWorldPosition(gridPos.x, gridPos.y)
                           + Vector3.one * (grid.CellSize / 2f);

        // guard: duplicate occupancy
        if (!grid.TryMarkOccupied(gridPos))
        {
            Debug.LogError($"Spawn clash at {gridPos}! Destroying self.");
            Destroy(gameObject);
        }

        // pass grid to UnitMover
        GetComponent<UnitMover>().Init(grid);
    }

    private void OnDestroy()
    {
        if (grid != null) grid.MarkUnoccupied(gridPos);
    }

    public void SetupFromData(PlayerData data)
    {
        dataRef = data;

        maxHP = data.maxHP;
        atk = data.atk;
        def = data.def;
        speed = data.speed;
        luck = data.luck;
        dex = data.dex;
        maxMovePoints = data.maxMovePoints;
    }

    public PlayerData ExtractToData()
    {
        if (dataRef == null) return null;
        dataRef.currentHP = maxHP;         
        return dataRef;
    }

    /* ------------------------------------
       6. Unity Update - input / selection
       ------------------------------------- */
    private void Update()
    {
        if (TurnManager.Instance == null || !TurnManager.Instance.IsBattleActive)
            return;
        if (HasActed) return;                            // already moved

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = AshenUtil.GridItems.GetMouseWorldPosition();
            grid.GetXY(mouseWorld, out int cx, out int cy);
            Vector2Int clicked = new Vector2Int(cx, cy);

            // click own tile: (de)select
            if (clicked == gridPos)
            {
                GridManager.Instance.SelectUnit(this);
                return;
            }

            // click highlighted tile -> move
            if (isSelected && IsHighlighted(clicked))
            {
                var path = PlayerPathfinder.FindPath(
                    grid, gridPos, clicked, maxMovePoints);

                if (path != null)
                    GetComponent<UnitMover>().MoveAlong(path);
            }
        }
    }

    /* 
       7. Selection helpers
       */
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

    /* -----------------------------------
       8. Move‑range generation
       --------------------------------- */
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
                if (c1 > maxMovePoints) continue;

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

    /* ---------------------------------------
       9. Public helpers
       ----------------------------------------- */
    public Vector2Int GetGridPosition() => gridPos;

    public bool HasMoveHighlight(Vector2Int tile)
    {
        return moveHighlights.Exists(obj =>
        {
            var s = obj.name.Split('_');
            return int.Parse(s[1]) == tile.x && int.Parse(s[2]) == tile.y;
        });
    }

}
