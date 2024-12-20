using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Player speed
    public float jumpForce = 10f; // Jump force
    public LayerMask groundLayer; // Ground layer for raycast detection

    private Rigidbody2D rb;
    private Vector2 boxSize = new Vector2(1.1f, 0.1f); // Small box at player's feet
    private float groundCheckOffset = 1.2f; // Distance to offset the box to player's feet
    private float groundCheckDistance = 0.05f; // Slight downward cast for accuracy

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontal Movement
        float move = Input.GetAxis("Horizontal");
        if (move != 0)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y); // Horizontal velocity
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    bool IsGrounded()
    {
        // Box positioned slightly below the player's feet
        Vector2 position = (Vector2)transform.position + Vector2.down * groundCheckOffset;

        // Perform BoxCast to detect ground
        RaycastHit2D hit = Physics2D.BoxCast(position, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayer);

        // Debug log for ground detection
        if (hit.collider != null)
        {
            Debug.Log("Ground detected: " + hit.collider.name);
        }

        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Visualize the box at player's feet
        Vector2 position = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        Gizmos.DrawWireCube(position, boxSize);
    }
}
