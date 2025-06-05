using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom2D : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

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