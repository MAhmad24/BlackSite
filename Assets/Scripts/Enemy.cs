using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public int health = 3;
    public int damage = 1;

    // Event that fires when enemy dies
    public System.Action OnDeath;

    private Transform player;
    private Rigidbody2D rb;

    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        // Chase the player
        ChasePlayer();
    }
    
    void ChasePlayer()
    {
        if (player == null) return; // Safety check
        
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Move toward player
        rb.linearVelocity = direction * moveSpeed;
    }
    
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        
        // Visual feedback (optional - we'll add this later)
        Debug.Log("Enemy hit! Health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Notify any listeners (WaveManager) that this enemy died
        if (OnDeath != null){
            OnDeath.Invoke();
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by bullet
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
            
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Enemy colliding with: " + collision.gameObject.name);
        // Check if touching player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Touching player! Attempting damage...");
            // Try to damage the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log("PlayerHealth component not found!");
            }
        }
    }
}