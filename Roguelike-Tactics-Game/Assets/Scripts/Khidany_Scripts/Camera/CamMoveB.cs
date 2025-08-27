using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Input")]
    public InputActionReference movementAction;  // Drag & drop your Move action here in inspector

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float boundaryMargin = 1f;


    public TestGrid gridRef;

    private Vector2Int gridSize;
    private Vector2 moveInput;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;

        gridSize = gridRef.GetGridSize();


        if (movementAction != null)
            movementAction.action.Enable();
    }

    private void OnEnable()
    {
        if (movementAction != null)
            movementAction.action.Enable();
    }

    private void OnDisable()
    {
        if (movementAction != null)
            movementAction.action.Disable();
    }

    private void Update()
    {
        if (movementAction != null)
            moveInput = movementAction.action.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero;

        Vector3 currentPos = cam.transform.position;
        Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0f) * moveSpeed * Time.deltaTime;

        Vector3 targetPos = currentPos;

        // Try move on X axis only
        Vector3 tryX = currentPos + new Vector3(delta.x, 0f, 0f);
        if (IsInsideBounds(tryX))
            targetPos.x = tryX.x;

        // Try move on Y axis only
        Vector3 tryY = currentPos + new Vector3(0f, delta.y, 0f);
        if (IsInsideBounds(tryY))
            targetPos.y = tryY.y;

        cam.transform.position = targetPos;
    }


    private void CenterCamera()
    {
        float camX = (gridSize.x - 1) / 2f;
        float camY = (gridSize.y - 1) / 2f;
        cam.transform.position = new Vector3(camX, camY, cam.transform.position.z);
    }

    private bool IsInsideBounds(Vector3 pos)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = halfWidth - boundaryMargin;
        float maxX = gridSize.x - halfWidth + boundaryMargin;
        float minY = halfHeight - boundaryMargin;
        float maxY = gridSize.y - halfHeight + boundaryMargin;

        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }



}
