using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom2D : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 10f;

    public float initialZoom = 5f;

    void Start()             
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // Clamp in case the inspector values are outside the limits
            cam.orthographicSize = Mathf.Clamp(initialZoom, minZoom, maxZoom);
        }
    }


    void Update()
    {
        // Zoom with mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Zoom with Q/E keys
        //if (Input.GetKey(KeyCode.Q)) scroll = 0.1f;
        //if (Input.GetKey(KeyCode.E)) scroll = -0.1f;

        if (scroll != 0f)
        {
            Camera cam = Camera.main;
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
}