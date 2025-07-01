using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    private PlayerData dataRef;

    public int maxHP = 20;
    public int atk = 5;
    public int def = 2;
    public int speed = 3;
    public int luck = 1;
    public int dex = 2;
    // public Sprite portait;

    public int maxMovePoints = 4;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject moveHighlightPrefab;

    private GridMap grid;
    private Vector2Int gridPos;
    private bool isSelected = false;
    private List<GameObject> moveHighlights = new List<GameObject>();

    public bool HasActed { get; private set; } = false;

    public void Init(GridMap g, Vector2Int startPos)
    {
        grid = g;
        gridPos = startPos;
        transform.position = grid.GetWorldPosition(gridPos.x, gridPos.y)
                         + Vector3.one * (grid.CellSize / 2f);

        // Abort if somebody is already standing here (should never happen, safeguards anyway)
        if (!grid.TryMarkOccupied(gridPos))
        {
            Debug.LogError($"Spawn clash at {gridPos}! Destroying self.");
            Destroy(gameObject);
        }
    }

    //  fire andforget safety net
    private void OnDestroy()
    {
        if (grid != null)
            grid.MarkUnoccupied(gridPos);
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

        dataRef.currentHP = maxHP; // Replace with actual HP if tracked elsewhere
        return dataRef;
    }


    private void Update()
    {
        if (!BattlePrepUIManager.IsBattleActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = AshenUtil.GridItems.GetMouseWorldPosition();
            grid.GetXY(mouseWorldPos, out int clickX, out int clickY);
            Vector2Int clickedPos = new Vector2Int(clickX, clickY);

            if (clickedPos == gridPos)
            {
                GridManager.Instance.SelectUnit(this);
                return;
            }

            if (isSelected && IsHighlighted(clickedPos))
            {
                StartCoroutine(MoveToPosition(clickedPos));
                GridManager.Instance.ClearSelectedUnit(); // Deselect after move
            }
        }
    }




    private void ShowMoveRange()
    {
        ClearMoveRange();
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!grid.IsInBounds(x, y)) continue;
                if (grid.IsOccupied(pos)) continue;

                int cost = grid.GetValue(x, y);
                int dist = Mathf.Abs(pos.x - gridPos.x) + Mathf.Abs(pos.y - gridPos.y);
                if (cost * dist <= maxMovePoints && cost > 0)
                {
                    Vector3 worldPos = grid.GetWorldPosition(x, y) + Vector3.one * (grid.CellSize / 2f);
                    var highlight = Instantiate(moveHighlightPrefab, worldPos, Quaternion.identity);
                    highlight.name = $"Highlight_{x}_{y}";
                    moveHighlights.Add(highlight);
                }
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


    private void ClearMoveRange()
    {
        foreach (var h in moveHighlights)
            if (h) Destroy(h);
        moveHighlights.Clear();
    }

    private bool IsHighlighted(Vector2Int pos)
    {
        return moveHighlights.Exists(obj =>
        {
            var split = obj.name.Split('_');
            return int.Parse(split[1]) == pos.x && int.Parse(split[2]) == pos.y;
        });
    }

    private IEnumerator MoveToPosition(Vector2Int targetPos)
    {
        grid.MarkUnoccupied(gridPos);
        Vector3 start = transform.position;
        Vector3 end = grid.GetWorldPosition(targetPos.x, targetPos.y) + Vector3.one * (grid.CellSize / 2f);

        float t = 0f;
        while (t < 0.5f)
        {
            transform.position = Vector3.Lerp(start, end, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        gridPos = targetPos;
        grid.MarkOccupied(gridPos);

        HasActed = true;
        TurnManager.Instance.NotifyUnitActed(); 

    }
    public void ResetTurn()
    {
        HasActed = false;
    }


    public Vector2Int GetGridPosition() => gridPos;
}
