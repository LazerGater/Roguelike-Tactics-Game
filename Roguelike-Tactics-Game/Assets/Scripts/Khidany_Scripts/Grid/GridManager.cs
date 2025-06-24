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
        var testScript = FindFirstObjectByType<TestScript>();
        grid = testScript?.Grid;

        if (grid == null)
        {
            Debug.LogError("GridManager: GridMap is null or TestScript missing.");
        }
    }

    // preview = true  -> units spawn as a visual preview (not yet in TurnManager)
    // preview = false -> final spawn; registers units and clears preview visuals
    public void SpawnSelectedParty(List<Vector2Int> spawnTiles, bool preview)
    {
        // 1. Wipe any existing preview or old battle units
        foreach (var u in previewUnits)
            if (u != null) Destroy(u.gameObject);
        previewUnits.Clear();
        playerUnits.Clear();

        previewActive = preview;

        var selected = PartyCarrier.Instance.playerParty.FindAll(d => d.isSelectedForBattle);
        int count = Mathf.Min(spawnTiles.Count, selected.Count);

        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = spawnTiles[i];
            PlayerData data = selected[i];

            GameObject go = Instantiate(playerPrefab);
            var unit = go.GetComponent<PlayerUnit>();
            unit.SetupFromData(data);
            unit.Init(grid, pos);              

            if (preview)
            {
                previewUnits.Add(unit);         // store separately
            }
            else
            {
                playerUnits.Add(unit);
                TurnManager.Instance.RegisterPlayerUnit(unit);
            }
        }
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
