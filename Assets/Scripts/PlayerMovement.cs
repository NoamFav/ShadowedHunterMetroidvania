using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Parameters")]
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float jumpForce = 10f;

    [SerializeField]
    private LayerMask groundLayer;

    // Ground detection parameters
    [Header("Ground Detection")]
    [SerializeField]
    private Vector2 groundBoxSize = new Vector2(0.8f, 0.1f);

    [SerializeField]
    private float groundCheckOffset = 0.5f;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    // Wall detection parameters
    [Header("Wall Detection")]
    [SerializeField]
    private Vector2 wallBoxSize = new Vector2(0.1f, 0.8f);

    [SerializeField]
    private float wallCheckOffset = 0.4f;

    [SerializeField]
    private float wallCheckDistance = 0.1f;

    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        Move();
        Jump();
        WallSlide();
        ClimbLedge();
    }

    void Move() =>
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.linearVelocity.y);

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void WallSlide()
    {
        if (IsTouchingWall() && !IsGrounded())
        {
            Debug.Log("Wall slide");
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -2f, float.MaxValue)
            );
        }
    }

    void ClimbLedge()
    {
        if (IsTouchingWall())
        {
            Vector2 climbCheckPos = (Vector2)transform.position + Vector2.up * 1.5f;
            bool canClimb = !Physics2D.Raycast(climbCheckPos, GetWallDirection(), 1f, groundLayer);

            if (canClimb)
            {
                Debug.Log("Climb ledge");
                var vector = new Vector2(rb.linearVelocity.x, jumpForce);

                rb.linearVelocity = vector;
            }
        }
    }

    bool IsGrounded()
    {
        Vector2 position = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        RaycastHit2D hit = Physics2D.BoxCast(
            position,
            groundBoxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        return hit.collider != null && hit.normal.y > 0.9f;
    }

    bool IsTouchingWall()
    {
        Vector2 direction = GetWallDirection();
        Vector2 position = (Vector2)transform.position + direction * wallCheckOffset;

        RaycastHit2D hit = Physics2D.BoxCast(
            position,
            wallBoxSize,
            0f,
            direction,
            wallCheckDistance,
            groundLayer
        );
        return hit.collider != null && Mathf.Abs(hit.normal.x) > 0.9f;
    }

    Vector2 GetWallDirection() => new Vector2(Mathf.Sign(rb.linearVelocity.x), 0);

    void OnDrawGizmos()
    {
        // Ground check
        Gizmos.color = Color.red;
        Vector2 groundPos = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        Gizmos.DrawWireCube(groundPos, groundBoxSize);

        // Wall check
        Gizmos.color = Color.blue;
        Vector2 wallPos = (Vector2)transform.position + GetWallDirection() * wallCheckOffset;
        Gizmos.DrawWireCube(wallPos, wallBoxSize);
    }
}
