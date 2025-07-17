using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform pointer;  // Assign this in inspector to the CameraPointer

    void LateUpdate()
    {
        if (pointer == null) return;

        Vector3 newPos = pointer.position;
        newPos.z = transform.position.z; // Maintain original Z
        transform.position = newPos;
    }
}
