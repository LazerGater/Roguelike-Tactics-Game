
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CameraBounds : MonoBehaviour
{
    [Tooltip("Extra space around the map in world units")]
    public float padding = 0f;

    PolygonCollider2D poly;

    void Awake() => poly = GetComponent<PolygonCollider2D>();

    // Call this once, right after the GridMap is ready.
    public void Build(GridMap grid)
    {
        // Build a simple rectangle path
        Vector2[] verts = new Vector2[4];
        float w = grid.width * grid.CellSize;
        float h = grid.height * grid.CellSize;

        verts[0] = new Vector2(-padding, -padding);
        verts[1] = new Vector2(-padding, h + padding);
        verts[2] = new Vector2(w + padding, h + padding);
        verts[3] = new Vector2(w + padding, -padding);

        poly.pathCount = 1;
        poly.SetPath(0, verts);

        // Put the collider so its lowerleft matches Grid origin
        transform.position = grid.GetWorldPosition(0, 0);
    }

    public PolygonCollider2D Collider => poly;
}
