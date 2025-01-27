using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Player speed
    public float jumpForce = 10f; // Jump force

    // I need to remove the layer logic no? cause the side of a ground is a wall? and the top of a wall is a ground?
    // I should base on the layer of the object that i hit, no?
    public LayerMask groundLayer; // Ground layer for raycast detection
    public LayerMask wallLayer; // Wall layer for raycast detection

    private Rigidbody2D rb;

    private Vector2 groundBoxSize = new Vector2(1f, 0.1f); // Box size for ground detection
    private float groundCheckOffset = 1f; // Distance to offset the box to player's feet
    private float groundCheckDistance = 0.05f; // Slight downward cast for accuracy

    private Vector2 wallBoxSize = new Vector2(.1f, 1.5f); // Box size for wall detection
    private float wallCheckOffset = 0.5f; // Distance to offset the box to player's slide
    private float wallCheckDistance = 0.01f; // Slight horizontal cast for accuracy

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump(); 
        WallSlide();
        ClimbLedge();
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal");

        if (IsGrounded())
        {
            // If grounded, ignore wall restrictions
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }
        else if (IsTouchingWall())
        {
            // If not grounded and touching a wall, stop horizontal movement
            move = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            // Normal horizontal movement
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void WallSlide()
    {
        if (IsTouchingWall() && !IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);  // Slow descent when wall sliding
        }
    }

    void ClimbLedge()
    {
        if (Input.GetButtonDown("Jump") && IsTouchingWall())
        {
            Vector2 climbCheckPos = (Vector2)transform.position + Vector2.up * 1.5f;
            RaycastHit2D hit = Physics2D.Raycast(climbCheckPos, Vector2.right * Mathf.Sign(rb.linearVelocity.x), 1f, groundLayer);

            if (hit.collider != null && Mathf.Abs(hit.normal.y) > 0.5f)  // Ensure there's ground above
            {
                Debug.Log("Ledge detected, climbing...");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    bool IsGrounded()
    {
        Vector2 position = (Vector2)transform.position + Vector2.down * groundCheckOffset;

        // Check both ground and wall layers
        RaycastHit2D hit = Physics2D.BoxCast(position, groundBoxSize, 0f, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null && Mathf.Abs(hit.normal.y) > 0.5f)  // Ensure surface is ground-like
        {
            Debug.Log("Landed on the ground: " + hit.collider.name);
            return true;
        }
        return false;
    }

    bool IsTouchingWall()
    {
        Vector2 position = (Vector2)transform.position + new Vector2((rb.linearVelocity.x > 0 ? 1 : -1) * wallCheckOffset, 0);
        Vector2 direction = rb.linearVelocity.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.BoxCast(position, wallBoxSize, 0f, direction, wallCheckDistance, groundLayer);

        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.5f)  // Ensure surface is wall-like
        {
            Debug.Log("Touching a wall: " + hit.collider.name);
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 position = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        Gizmos.DrawWireCube(position, groundBoxSize);

        Gizmos.color = Color.blue;
        position = (Vector2)transform.position + new Vector2((rb.linearVelocity.x > 0 ? 1 : -1) * wallCheckOffset, 0);
        Gizmos.DrawWireCube(position, wallBoxSize);
    }
}
