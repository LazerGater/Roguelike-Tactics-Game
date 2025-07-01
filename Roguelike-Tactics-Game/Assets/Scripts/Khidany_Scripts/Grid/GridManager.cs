using System.Collections.Generic;
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

    private bool previewActive = false;
    private readonly List<PlayerUnit> previewUnits = new List<PlayerUnit>();

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
        // ---------- 0. Housekeeping ----------
        void DespawnList(List<PlayerUnit> list)
        {
            foreach (var u in list)
                if (u != null)
                {
                    grid.MarkUnoccupied(u.GetGridPosition());  // safety
                    Destroy(u.gameObject);
                }
            list.Clear();
        }

        DespawnList(previewUnits);
        if (!preview) DespawnList(playerUnits);

        previewActive = preview;

        // ---------- 1. Build work lists ----------
        var chosenUnits = PartyCarrier.Instance.playerParty
                           .FindAll(d => d.isSelectedForBattle);

        // Make a compact list of spawn tiles (remove duplicates)
        var uniqueTiles = new List<Vector2Int>();
        var dupeCheck = new HashSet<Vector2Int>();
        foreach (var t in spawnTiles)
            if (dupeCheck.Add(t)) uniqueTiles.Add(t);
            else Debug.LogWarning($"Duplicate spawn tile {t} in map data - ignored.");

        // ---------- 2. Spawn loop ----------
        var taken = new HashSet<Vector2Int>();   // guarantees 1-to-1
        int pairs = Mathf.Min(uniqueTiles.Count, chosenUnits.Count);

        for (int i = 0; i < pairs; i++)
        {
            Vector2Int pos = uniqueTiles[i];

            if (taken.Contains(pos) || grid.IsOccupied(pos))
            {
                Debug.LogError($"Tile {pos} already taken at spawn time - unit skipped.");
                continue;
            }

            taken.Add(pos);

            GameObject go = Instantiate(playerPrefab);
            var unit = go.GetComponent<PlayerUnit>();
            unit.SetupFromData(chosenUnits[i]);
            unit.Init(grid, pos);

            if (preview)
                previewUnits.Add(unit);
            else
            {
                playerUnits.Add(unit);
                TurnManager.Instance.RegisterPlayerUnit(unit);
            }
        }

        // ---------- 3. Postspawn diagnostics ----------
        if (chosenUnits.Count > pairs)
            Debug.LogWarning($"Only {pairs} of {chosenUnits.Count} selected units could be placed.");
        if (uniqueTiles.Count > pairs)
            Debug.LogWarning($"Map had {uniqueTiles.Count} spawn tiles but only {pairs} were needed.");
    }

    public void SelectUnit(PlayerUnit unit)
    {
        if (selectedUnit != null && selectedUnit != unit)
        {
            selectedUnit.Deselect();
        }

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
