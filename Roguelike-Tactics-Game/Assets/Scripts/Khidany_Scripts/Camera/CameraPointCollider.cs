using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CameraPointer : MonoBehaviour
{
    public Camera cam;

    public Vector2 boxSize = new Vector2(0.2f, 0.2f);

    private Rigidbody2D rb;
    private BoxCollider2D box;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        // Detach so physics moves it independently
        transform.parent = null;

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        box = GetComponent<BoxCollider2D>();
        box.size = boxSize;
        box.isTrigger = false;
    }

    void FixedUpdate()
    {
        if (cam == null) return;

        // Camera center in world space
        Vector3 cameraCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector2 target = new Vector2(cameraCenter.x, cameraCenter.y);

        // Clamp to wall bounds
        Vector2 clamped = ClampToBounds(target);

        rb.MovePosition(clamped);
    }
    Vector2 ClampToBounds(Vector2 target)
    {
        // Define your clamp limits based on the grid + wall thickness
        float minX = GridOverlay.MinX + boxSize.x * 0.5f;
        float maxX = GridOverlay.MaxX - boxSize.x * 0.5f;
        float minY = GridOverlay.MinY + boxSize.y * 0.5f;
        float maxY = GridOverlay.MaxY - boxSize.y * 0.5f;

        float x = Mathf.Clamp(target.x, minX, maxX);
        float y = Mathf.Clamp(target.y, minY, maxY);
        return new Vector2(x, y);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Pointer] ENTERED wall: {collision.gameObject.name} at {collision.contacts[0].point}");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"[Pointer] STAYING on wall: {collision.gameObject.name}");
    }

    void OnDrawGizmos()
    {
        if (box == null) box = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.yellow;
        Vector3 size = box != null
            ? new Vector3(box.size.x, box.size.y, 0f)
            : new Vector3(0.2f, 0.2f, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
