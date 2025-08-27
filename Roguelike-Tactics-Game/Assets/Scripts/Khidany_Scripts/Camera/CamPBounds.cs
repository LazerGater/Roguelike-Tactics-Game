using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementLimiterInput : MonoBehaviour
{
    [Header("Grid Reference")]
    [SerializeField] private TestGrid gridRef;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
    private Vector2Int gridSize;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        if (gridRef != null)
            gridSize = gridRef.GetGridSize();

        // Start centered
        Vector3 center = new Vector3((gridSize.x - 1) / 2f, (gridSize.y - 1) / 2f, -10f);
        transform.position = center;
    }

    private void Update()
    {
        if (moveInput == Vector2.zero) return;

        Vector3 move = (Vector3)(moveInput.normalized * moveSpeed * Time.deltaTime);
        Vector3 nextPos = transform.position + move;

        if (IsInsideBounds(nextPos))
        {
            transform.position = nextPos;
        }
        else
        {
            // Try X-only or Y-only for slide-along-wall feel
            Vector3 xOnly = new Vector3(nextPos.x, transform.position.y, transform.position.z);
            Vector3 yOnly = new Vector3(transform.position.x, nextPos.y, transform.position.z);

            if (IsInsideBounds(xOnly))
                transform.position = xOnly;
            else if (IsInsideBounds(yOnly))
                transform.position = yOnly;
        }
    }

    private bool IsInsideBounds(Vector3 pos)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = halfWidth;
        float maxX = gridSize.x - halfWidth;
        float minY = halfHeight;
        float maxY = gridSize.y - halfHeight;

        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }


    // Input System binding: look for "Move" Vector2 action
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
}
