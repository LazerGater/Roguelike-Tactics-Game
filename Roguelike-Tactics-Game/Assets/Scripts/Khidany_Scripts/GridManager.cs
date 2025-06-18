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
            return;
        }

        foreach (Vector2Int pos in spawnPositions)
        {
            if (!grid.IsInBounds(pos.x, pos.y)) continue;

            GameObject unitGO = Instantiate(playerPrefab);
            PlayerUnit unit = unitGO.GetComponent<PlayerUnit>();
            if (unit != null)
            {
                unit.Init(grid, pos);
                playerUnits.Add(unit);
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
