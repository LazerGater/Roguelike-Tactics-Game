using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GridInitializer gridInit;
    private float minX, maxX, minY, maxY;
    private float cameraHalfWidth, cameraHalfHeight;

    private void Start()
    {
        if (gridInit == null)
            gridInit = FindFirstObjectByType<GridInitializer>();

        if (gridInit != null)
        {
            GridMap grid = gridInit.Grid;
            float w = grid.width * grid.CellSize;
            float h = grid.height * grid.CellSize;
            Vector3 center = new Vector3(w * 0.5f, h * 0.5f, transform.position.z);
            transform.position = center;

            Camera cam = Camera.main;
            cameraHalfHeight = cam.orthographicSize;
            cameraHalfWidth = cameraHalfHeight * cam.aspect;

            // Pull limits from GridOverlay
            minX = GridOverlay.MinX + cameraHalfWidth;
            maxX = GridOverlay.MaxX - cameraHalfWidth;
            minY = GridOverlay.MinY + cameraHalfHeight;
            maxY = GridOverlay.MaxY - cameraHalfHeight;
        }
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 delta = new Vector3(h, v, 0f) * moveSpeed * Time.deltaTime;
        Vector3 next = transform.position + delta;

        // Clamp camera so center stays inside bounds
        next.x = Mathf.Clamp(next.x, minX, maxX);
        next.y = Mathf.Clamp(next.y, minY, maxY);

        transform.position = next;
    }
}
