using System.Collections;
using System.IO;  
using System.Collections.Generic;
using UnityEngine;
using AshenUtil;
public class TestScript : MonoBehaviour
{
    private GridMap grid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        int[,] savedValues = SavedGridData.GridValues;

        int width = savedValues.GetLength(1); // Columns
        int height = savedValues.GetLength(0); // Rows
        float cellSize = 5f;
        Vector3 originPosition = new Vector3(5, 5);

        grid = new GridMap(width, height, cellSize, originPosition);

        // Fill grid with saved values, flipped to match Unity bottom left
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int flippedY = height - 1 - y;
                grid.SetValue(x, flippedY, savedValues[y, x]);
            }
        }
        //  grid = new GridMap(30, 30, 5f, new Vector3( 5, 5)); // For creating new grid
    }
    private void SaveGridToScript()
    {
        var width = grid.width;
        var height = grid.height;

        string filePath = Application.dataPath + "/SavedGrid.cs";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("public static class SavedGridData");
            writer.WriteLine("{");
            writer.WriteLine("    public static readonly int[,] GridValues = new int[,] {");

            for (int y = grid.height - 1; y >= 0; y--)
            {
                writer.Write("        { ");
                for (int x = 0; x < grid.width; x++)
                {
                    writer.Write(grid.GetValue(x, y));
                    if (x < grid.width - 1) writer.Write(", ");
                }
                writer.WriteLine(" },");
            }

            writer.WriteLine("    };");
            writer.WriteLine("}");
        }

        Debug.Log("Grid saved to " + filePath);
    }

    private void Update()
    {
        Vector3 mouseWorldPos = GridItems.GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            int currentValue = grid.GetValue(mouseWorldPos);
            grid.SetValue(mouseWorldPos, currentValue + 1);
        }

        if (Input.GetMouseButtonDown(1)) // Right click
        {
            int currentValue = grid.GetValue(mouseWorldPos);
            int newValue = Mathf.Max(0, currentValue - 1);
            grid.SetValue(mouseWorldPos, newValue);
        }

        //if (Input.GetKeyDown(KeyCode.S)) // Press S to save
        //{
        //    SaveGridToScript();
        //}
    }




}
