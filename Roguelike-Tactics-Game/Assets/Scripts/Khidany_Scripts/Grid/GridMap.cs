using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AshenUtil;

public class GridMap
{
    [SerializeField] private GameObject tilePrefab;

    public int width;
    public int height;
    public float cellSize;
    private Vector3 originPosition;

    private int[,] gridArray;
    private TextMesh[,] debugArray;

    // New: Store occupied tiles
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    // Public accessor for cellSize
    public float CellSize => cellSize;

    public GridMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        debugArray = new TextMesh[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = 1;
                debugArray[x, y] = GridItems.CreateWorldText(
                   gridArray[x, y].ToString(), null,
                   GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f,
                   20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center
               );

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public int GetMovementCost(int x, int y)
    {
        return IsInBounds(x, y) ? gridArray[x, y] : int.MaxValue;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = Mathf.Max(1, value);

            debugArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    // NEW: Occupancy handling
    public void MarkOccupied(Vector2Int pos)
    {
        if (IsInBounds(pos.x, pos.y)) occupiedPositions.Add(pos);
    }

    public void MarkUnoccupied(Vector2Int pos)
    {
        occupiedPositions.Remove(pos);
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return occupiedPositions.Contains(pos);
    }
    public bool TryMarkOccupied(Vector2Int pos)
    {
        if (!IsInBounds(pos.x, pos.y)) return false;
        return occupiedPositions.Add(pos);   // HashSet.Add returns false if duplicate
    }


}
