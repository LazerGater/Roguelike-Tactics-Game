using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AshenUtil;
using System.Linq;

public class GridInitializer : MonoBehaviour
{
    private GridMap grid;
    public GridMap Grid => grid;

    private List<Vector2Int> allySpawns = new List<Vector2Int>();
    public List<Vector2Int> AllySpawns => allySpawns;
    public int PartyLimit { get; private set; }

    [SerializeField] private int cellSize = 5;
    [SerializeField] private GameObject cellVisualPrefab;

    private void Start()
    {
        MapData map = MapSelector.GetRandomMap();   // now returns MapData
        int[,] savedValues = map.gridValues;        // movement cost array
        allySpawns = new List<Vector2Int>(map.allySpawns); // copy spawn list
        PartyLimit =map.partyLimit;

        var dupes = allySpawns
            .GroupBy(p => p)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (dupes.Count > 0)
            Debug.LogError($"Map has duplicate ally spawn tiles: {string.Join(", ", dupes)}");

        int width = savedValues.GetLength(1);
        int height = savedValues.GetLength(0);
        Vector3 originPosition = Vector3.zero;

        grid = new GridMap(width, height, cellSize, originPosition);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int flippedY = height - 1 - y;
                grid.SetValue(x, flippedY, savedValues[y, x]);

                Vector3 basePos = grid.GetWorldPosition(x, flippedY);
                Vector3 half = new Vector3(cellSize, cellSize, 0) * 0.5f;
                Instantiate(cellVisualPrefab, basePos + half, Quaternion.identity, transform)
                    .name = $"Cell_{x}_{flippedY}";
            }
        }
        Debug.Log($"Ally spawns for this map: {string.Join(", ", allySpawns)}");

    }

    private void Update()
    {
        Vector3 mouseWorldPos = GridItems.GetMouseWorldPosition();

        //if (Input.GetMouseButtonDown(0))
        //{
        //    int currentValue = grid.GetValue(mouseWorldPos);
        //    grid.SetValue(mouseWorldPos, currentValue + 1);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    int currentValue = grid.GetValue(mouseWorldPos);
        //    int newValue = Mathf.Max(0, currentValue - 1);
        //    grid.SetValue(mouseWorldPos, newValue);
        //}
    }

    //private void SaveGridToClipboard()
    //{
    //    int[,] gridValues = grid.ExportGridValues();
    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    //    sb.AppendLine("public static class SavedGridData\n{");
    //    sb.AppendLine("    public static readonly int[,] GridValues = new int[,] {");

    //    int height = gridValues.GetLength(0);
    //    int width = gridValues.GetLength(1);
    //    for (int y = 0; y < height; y++)
    //    {
    //        sb.Append("        { ");
    //        for (int x = 0; x < width; x++)
    //        {
    //            sb.Append(gridValues[y, x]);
    //            if (x < width - 1) sb.Append(", ");
    //        }
    //        sb.AppendLine(" }" + (y < height - 1 ? "," : ""));
    //    }

    //    sb.AppendLine("    };");
    //    sb.AppendLine("}");

    //    GUIUtility.systemCopyBuffer = sb.ToString();
    //    Debug.Log("Grid saved to clipboard!");
    //}

}
