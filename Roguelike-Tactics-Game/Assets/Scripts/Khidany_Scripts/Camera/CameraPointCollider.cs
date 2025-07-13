using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CameraPointer : MonoBehaviour
{
    public Camera cam;
    public float moveSpeed = 5f;
    public Vector2 boxSize = new Vector2(0.2f, 0.2f);

    private Rigidbody2D rb;
    private BoxCollider2D box;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

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
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 delta = input * moveSpeed * Time.fixedDeltaTime;

        Vector2 target = rb.position + delta;
        Vector2 clamped = ClampToBounds(target);

        rb.MovePosition(clamped);

        // Keep camera centered on pointer
        if (cam != null)
        {
            Vector3 camPos = transform.position;
            camPos.z = cam.transform.position.z;
            cam.transform.position = camPos;
        }
    }

    Vector2 ClampToBounds(Vector2 target)
    {
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