using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    [Header("Grid dimensions (cells)")]
    public int width = 10;
    public int height = 10;

    [Header("Scale and origin")]
    public float cellSize = 1f;
    public Vector3 originPosition = Vector3.zero;

    [Header("Line appearance")]
    public float lineWidth = 0.02f;
    public Color lineColor = new Color(1f, 1f, 1f, 0.25f);
    public Material lineMaterial;           // optional

    [Header("Camera Wall")]
    [SerializeField] private GameObject cameraWallPrefab;
    [SerializeField] private float wallThickness = 2f;
    public static float MinX, MaxX, MinY, MaxY;

    readonly List<LineRenderer> lines = new List<LineRenderer>();

    /* ---------------- Runtime entry ---------------- */
    void Start()
    {
        Build();                            // draw once when scene starts
    }

    /* Called by GridInitializer after it knows GridMap */
    public void BuildFromGrid(GridMap grid)
    {
        width = grid.width;
        height = grid.height;
        cellSize = grid.CellSize;
        originPosition = grid.GetWorldPosition(0, 0);
        Build();
        BuildCameraBounds();
    }

    /* ---------------- Core builder ----------------- */
    void Build()
    {
        int needed = width + height + 2;
        EnsurePool(needed);

        int i = 0;

        // Vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 p0 = originPosition + new Vector3(x * cellSize, 0f);
            Vector3 p1 = p0 + new Vector3(0f, height * cellSize);
            ApplyLine(lines[i++], p0, p1);
        }

        // Horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 p0 = originPosition + new Vector3(0f, y * cellSize);
            Vector3 p1 = p0 + new Vector3(width * cellSize, 0f);
            ApplyLine(lines[i++], p0, p1);
        }
    }


    public void BuildCameraBounds()
    {
        Vector3 gridSize = new Vector3(width, height) * cellSize;
        Vector3 center = originPosition + gridSize * 0.5f;
        Vector3 min = center - gridSize * 0.5f;
        Vector3 max = center + gridSize * 0.5f;
        float t = wallThickness;

        MinX = min.x - t;
        MaxX = max.x + t;
        MinY = min.y - t;
        MaxY = max.y + t;

        // Top wall
        CreateWall("Top",
            new Vector2(center.x, max.y + t * 0.5f),
            new Vector2(gridSize.x, t));

        // Bottom wall
        CreateWall("Bottom",
            new Vector2(center.x, min.y - t * 0.5f),
            new Vector2(gridSize.x, t));

        // Left wall
        CreateWall("Left",
            new Vector2(min.x - t * 0.5f, center.y),
            new Vector2(t, gridSize.y));

        // Right wall
        CreateWall("Right",
            new Vector2(max.x + t * 0.5f, center.y),
            new Vector2(t, gridSize.y));
    }

    void CreateWall(string side, Vector2 center, Vector2 size)
    {
        GameObject wall = Instantiate(cameraWallPrefab, center, Quaternion.identity, transform);
        wall.name = $"CameraWall_{side}";
        BoxCollider2D col = wall.GetComponent<BoxCollider2D>();
        col.size = size;
    }

    void OnDrawGizmos()
    {
        // Grid bounds (red)
        Vector3 size = new Vector3(width, height) * cellSize;
        Vector3 center = originPosition + size * 0.5f;

        // Wall preview (cyan)
        float t = wallThickness;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(center.x, center.y + size.y / 2 + t / 2), new Vector3(size.x, t, 0.1f));
        Gizmos.DrawCube(new Vector3(center.x, center.y - size.y / 2 - t / 2), new Vector3(size.x, t, 0.1f));
        Gizmos.DrawCube(new Vector3(center.x - size.x / 2 - t / 2, center.y), new Vector3(t, size.y, 0.1f));
        Gizmos.DrawCube(new Vector3(center.x + size.x / 2 + t / 2, center.y), new Vector3(t, size.y, 0.1f));
    }

    /* -------- helpers -------- */
    void EnsurePool(int count)
    {
        while (lines.Count < count)
        {
            var go = new GameObject("Line");
            go.transform.SetParent(transform, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.material = lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
            lr.sortingLayerName = "Grid";
            lr.sortingOrder = 0;

            lines.Add(lr);
        }
    }

    void ApplyLine(LineRenderer lr, Vector3 a, Vector3 b)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.widthMultiplier = lineWidth;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
    }

}