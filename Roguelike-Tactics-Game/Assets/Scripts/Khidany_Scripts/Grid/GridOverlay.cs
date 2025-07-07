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