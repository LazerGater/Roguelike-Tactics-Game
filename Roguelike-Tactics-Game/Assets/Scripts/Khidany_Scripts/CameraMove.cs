using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A / D
        float vertical = Input.GetAxis("Vertical");     // W / S

        Vector3 moveDirection = new Vector3(horizontal, vertical, 0f);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
