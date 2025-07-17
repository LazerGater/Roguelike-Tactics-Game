using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Vector2Int> spawnPositions = new List<Vector2Int>();

    private GridMap grid;
    private List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    private PlayerUnit selectedUnit = null;

    public List<PlayerUnit> GetPlayerUnits() => playerUnits;
    public void HighlightTile(Vector2Int pos) { /* Instantiate overlay */ }
    public void ClearTileHighlight(Vector2Int pos) { /* Destroy overlays */ }


    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        var testScript = FindFirstObjectByType<GridInitializer>();
        grid = testScript?.Grid;

        if (grid == null)
        {
            Debug.LogError("GridManager: GridMap is null or TestScript missing.");
        }
    }

    public void SpawnSelectedParty(List<Vector2Int> spawnTiles, bool preview)
    {
        // preview flag ignored (always false in BattleScene)
        void Despawn(List<PlayerUnit> list)
        {
            foreach (var u in list)
            {
                if (u != null)
                {
                    grid.MarkUnoccupied(u.GetGridPosition());
                    Destroy(u.gameObject);
                }
            }
            list.Clear();
        }
        Despawn(playerUnits);

        // get the top N selected units
        var chosen = PartyCarrier.Instance.playerParty
            .OrderBy(d => d.priorityID)
            .Take(FindFirstObjectByType<GridInitializer>().PartyLimit)
            .ToList();

        // dedupe and spawn
        var uniqueTiles = spawnTiles.Distinct().ToList();
        int count = Mathf.Min(uniqueTiles.Count, chosen.Count);
        var taken = new HashSet<Vector2Int>();

        for (int i = 0; i < count; i++)
        {
            var pos = uniqueTiles[i];
            if (taken.Contains(pos) || grid.IsOccupied(pos)) continue;
            taken.Add(pos);

            var go = Instantiate(playerPrefab);
            var unit = go.GetComponent<PlayerUnit>();
            unit.SetupFromData(chosen[i]);
            unit.Init(grid, pos);
            playerUnits.Add(unit);
            TurnManager.Instance.RegisterPlayerUnit(unit);
        }
    }


    public void SelectUnit(PlayerUnit unit)
    {
        if (unit.HasActed) return;               
        if (selectedUnit != null && selectedUnit != unit)
            selectedUnit.Deselect();

        selectedUnit = unit;
        selectedUnit.Select();
    }

    public void ClearSelectedUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.Deselect();
            selectedUnit = null;
        }
    }
}
