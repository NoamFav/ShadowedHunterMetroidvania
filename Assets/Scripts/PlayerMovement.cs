using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Player speed
    public float jumpForce = 10f; // Jump force
    public LayerMask groundLayer; // Ground layer for raycast detection

    private Rigidbody2D rb;

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
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    bool IsGrounded()
{
    Vector2 position = transform.position;
    Vector2 direction = Vector2.down;
    float distance = 1.5f;

    RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);

    if (hit.collider != null)
    {
        Debug.Log($"Hit {hit.collider.name}");
    }
    else
    {
        Debug.Log("No ground detected");
    }

    return hit.collider != null;
}

    void OnDrawGizmos()
    {
        // Visualize the ground check in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.5f);
    }
}
