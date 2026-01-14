using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public int health = 3;
    public int damage = 1;
    
    public System.Action OnDeath;
    
    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        if (!isDead)
        {
            ChasePlayer();
        }
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // CRITICAL: Stop if already dead
        
        health -= damageAmount;
        Debug.Log("Enemy hit! Health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (isDead) return; // CRITICAL: Prevent double-death
        isDead = true;
        
        Debug.Log("=== ENEMY DIE() CALLED ===");
        
        // CRITICAL: Disable colliders IMMEDIATELY so bullets stop hitting
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        // Stop movement
        rb.linearVelocity = Vector2.zero;
        
        // Notify WaveManager
        if (OnDeath != null)
        {
            OnDeath.Invoke();
            OnDeath = null;
        }
        
        // Destroy after brief delay (gives Die() time to complete)
        Destroy(gameObject, 0.1f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
