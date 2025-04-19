using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CapsulePlayer2DController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField]
    private float wallCheckDistance = 0.1f;

    // Components
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    // State
    private bool isGrounded;
    private bool isTouchingWall;
    private float moveDirection = 0f;

    private void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // Configure rigidbody
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Set up capsule collider for a 1x2 size capsule
        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.direction = CapsuleDirection2D.Vertical;

        // Set up the ground layer mask
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        // Check environmental collisions
        CheckGrounded();
        CheckWalls();

        // Handle input
        HandleMovementInput();

        // Handle jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        // Apply movement
        ApplyMovement();
    }

    private void CheckGrounded()
    {
        // Calculate the bottom position of the capsule
        Vector2 capsuleBottom =
            (Vector2)transform.position - new Vector2(0, capsuleCollider.size.y / 2f);

        // Check if the capsule is grounded with a small raycast
        RaycastHit2D hit = Physics2D.Raycast(
            capsuleBottom,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isGrounded = hit.collider != null;

        // Additional ground check using circle cast for more precision
        if (!isGrounded)
        {
            float radius = capsuleCollider.size.x / 2f;
            isGrounded = Physics2D.CircleCast(
                capsuleBottom,
                radius,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );
        }
    }

    private void CheckWalls()
    {
        // Get the capsule width
        float capsuleWidth = capsuleCollider.size.x / 2f;

        // Calculate the left and right positions for wall checks
        Vector2 capsuleRight = (Vector2)transform.position + new Vector2(capsuleWidth, 0);
        Vector2 capsuleLeft = (Vector2)transform.position - new Vector2(capsuleWidth, 0);

        // Check if touching walls on either side
        RaycastHit2D hitRight = Physics2D.Raycast(
            capsuleRight,
            Vector2.right,
            wallCheckDistance,
            groundLayer
        );
        RaycastHit2D hitLeft = Physics2D.Raycast(
            capsuleLeft,
            Vector2.left,
            wallCheckDistance,
            groundLayer
        );

        isTouchingWall = (hitRight.collider != null || hitLeft.collider != null);
    }

    private void HandleMovementInput()
    {
        // Get horizontal input
        moveDirection = Input.GetAxis("Horizontal");
    }

    private void Jump()
    {
        // Apply jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void ApplyMovement()
    {
        // Check if touching wall while trying to move into it
        if (isTouchingWall)
        {
            // Check direction of wall collision
            Vector2 capsuleRight =
                (Vector2)transform.position + new Vector2(capsuleCollider.size.x / 2f, 0);
            Vector2 capsuleLeft =
                (Vector2)transform.position - new Vector2(capsuleCollider.size.x / 2f, 0);

            RaycastHit2D hitRight = Physics2D.Raycast(
                capsuleRight,
                Vector2.right,
                wallCheckDistance,
                groundLayer
            );
            RaycastHit2D hitLeft = Physics2D.Raycast(
                capsuleLeft,
                Vector2.left,
                wallCheckDistance,
                groundLayer
            );

            // Prevent movement into the wall
            if (
                (hitRight.collider != null && moveDirection > 0)
                || (hitLeft.collider != null && moveDirection < 0)
            )
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
        }

        // Calculate and apply velocity
        float targetVelocityX = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        if (capsuleCollider == null)
            return;

        // Debug visualization for ground check
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 capsuleBottom =
            (Vector2)transform.position - new Vector2(0, capsuleCollider.size.y / 2f);
        Gizmos.DrawWireSphere(capsuleBottom, groundCheckDistance);

        // Debug visualization for wall checks
        Gizmos.color = isTouchingWall ? Color.blue : Color.yellow;
        float width = capsuleCollider.size.x / 2f;
        Vector2 capsuleRight = (Vector2)transform.position + new Vector2(width, 0);
        Vector2 capsuleLeft = (Vector2)transform.position - new Vector2(width, 0);
        Gizmos.DrawLine(capsuleRight, capsuleRight + new Vector2(wallCheckDistance, 0));
        Gizmos.DrawLine(capsuleLeft, capsuleLeft - new Vector2(wallCheckDistance, 0));
    }
}
