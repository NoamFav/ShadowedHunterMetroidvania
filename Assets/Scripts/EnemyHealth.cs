using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3; // Enemy health

    public void TakeDamage(int damage)
    {
        health -= damage; // Subtract damage from health
        Debug.Log(gameObject.name + " takes " + damage + " damage. Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject); // Destroy the enemy object
    }
}
