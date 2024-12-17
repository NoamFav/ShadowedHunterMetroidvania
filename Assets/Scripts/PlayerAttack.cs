using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint; // Point where the attack originates
    public float attackRange = 0.5f; // Range of the attack
    public LayerMask enemyLayer; // Layer to detect enemies
    public int attackDamage = 1; // Damage per attack
    public float attackRate = 2f; // Attacks per second
    private Animator animator;

    private float attackCooldown = 0f; // Cooldown timer

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Handle attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Attack input check
        if (Input.GetButtonDown("Fire1") && attackCooldown <= 0)
        {
            Attack();
            attackCooldown = 1f / attackRate; // Reset cooldown
        }
    }

    void Attack()
{
    if (attackPoint == null)
    {
        Debug.LogError("AttackPoint is not assigned!");
        return;
    }

    animator.SetTrigger("Attack"); // Trigger attack animation

    // OverlapCircle to detect enemies
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

    Debug.Log("AttackPoint Position: " + attackPoint.position);
    Debug.Log("Number of enemies detected: " + hitEnemies.Length);

    foreach (Collider2D enemy in hitEnemies)
    {
        Debug.Log("Hit Enemy: " + enemy.name);
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("No EnemyHealth script found on " + enemy.name);
        }
    }
}

    void OnDrawGizmosSelected()
{
    if (attackPoint == null)
    {
       Debug.LogWarning("AttackPoint is not assigned!");
        return;
    }

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
}
}
