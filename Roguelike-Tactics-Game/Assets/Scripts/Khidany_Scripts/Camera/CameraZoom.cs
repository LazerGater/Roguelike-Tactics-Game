using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom2D : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference zoomAction; 

    [Header("Settings")]
    public float zoomSpeed = 5f;
    public float padding = 1f;

    private float minZoom = 2f;
    private float maxZoom = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (zoomAction != null)
            zoomAction.action.Enable();
    }

    void OnDisable()
    {
        if (zoomAction != null)
            zoomAction.action.Disable();
    }

    // Called once by GridInitializer after it builds the map
    public void ConfigureZoom(GridMap grid)
    {
        float boardW = grid.width * grid.CellSize;
        float boardH = grid.height * grid.CellSize;

        // Fit whole board inside view  choose larger of half height and half width/aspect
        float halfH = boardH * 0.5f + padding;
        float halfW = boardW * 0.5f / cam.aspect + padding;
        maxZoom = Mathf.Max(halfH, halfW);

        minZoom = Mathf.Max(grid.CellSize * 0.6f, 1f);

        // Start centred and half zoomed
        cam.orthographicSize = Mathf.Clamp(maxZoom * 0.5f, minZoom, maxZoom);
    }

    void Update()
    {
        if (zoomAction == null) return;

        float scroll = zoomAction.action.ReadValue<float>(); // New Input System
        if (Mathf.Abs(scroll) < 0.01f) return;

        float newSize = cam.orthographicSize - scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}