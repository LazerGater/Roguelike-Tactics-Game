using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;

    private Dictionary<Vector2Int, Tile> grid = new();

    private void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        grid = new Dictionary<Vector2Int, Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} - {y} ";

                grid[new Vector2Int(x, y)] = spawnedTile;
            }
        }
        cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height /2 -0.5f, -10);
    }

    public Tile GetTileAtPos(Vector2Int pos)
    {
        if(grid.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Vector2Int GetGridSize()
    {
        return new Vector2Int(width, height);
    }
}
