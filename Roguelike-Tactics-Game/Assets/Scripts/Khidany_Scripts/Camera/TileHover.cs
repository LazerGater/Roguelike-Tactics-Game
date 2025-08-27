using UnityEngine;
using UnityEngine.InputSystem;

public class TileHover: MonoBehaviour
{
    [SerializeField] private TestGrid gridRef;
    [SerializeField] private float moveDelay = 0.0f;

    private Vector2Int hoverPos = new Vector2Int(0, 0);
    private Vector2Int gridSize;
    private float moveCooldown;

    private void Start()
    {
        gridSize = gridRef.GetGridSize(); // Dynamically fetch size
        UpdateHighlight();
    }

    private void Update()
    {
        if (moveCooldown > 0f)
        {
            moveCooldown -= Time.deltaTime;
            return;
        }

        Vector2 moveInput = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
        Vector2Int dir = Vector2Int.zero;

        if (moveInput.x > 0.5f) dir = Vector2Int.right;
        else if (moveInput.x < -0.5f) dir = Vector2Int.left;
        else if (moveInput.y > 0.5f) dir = Vector2Int.up;
        else if (moveInput.y < -0.5f) dir = Vector2Int.down;

        if (dir != Vector2Int.zero)
        {
            MoveHover(dir);
            moveCooldown = moveDelay;
        }
    }

    private void MoveHover(Vector2Int dir)
    {
        var oldTile = gridRef.GetTileAtPos(hoverPos);
        if (oldTile != null)
            oldTile.SetHighlight(false);

        hoverPos += dir;
        hoverPos.x = Mathf.Clamp(hoverPos.x, 0, gridSize.x - 1);
        hoverPos.y = Mathf.Clamp(hoverPos.y, 0, gridSize.y - 1);

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        var tile = gridRef.GetTileAtPos(hoverPos);
        if (tile != null)
            tile.SetHighlight(true);
    }
}
