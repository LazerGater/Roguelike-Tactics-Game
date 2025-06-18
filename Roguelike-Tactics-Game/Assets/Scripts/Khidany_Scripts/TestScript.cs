using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AshenUtil;

public class TestScript : MonoBehaviour
{
    private GridMap grid;
    public GridMap Grid => grid;

    [SerializeField] private int cellSize = 5;
    // New: assign a simple square prefab (1×1 unit) in inspector
    [SerializeField] private GameObject cellVisualPrefab;

    private void Start()
    {
        int[,] savedValues = MapSelector.GetRandomMap();

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

                // Instantiate a permanent cell visual at each grid position
                Vector3 basePos = grid.GetWorldPosition(x, flippedY);
                Vector3 halfOffset = new Vector3(cellSize, cellSize, 0) * 0.5f;
                Vector3 cellWorldPos = basePos + halfOffset;

                GameObject cellGO = Instantiate(cellVisualPrefab, cellWorldPos, Quaternion.identity, transform);
                // Optional: name it for clarity
                cellGO.name = $"Cell_{x}_{flippedY}";
            }
        }
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
}
