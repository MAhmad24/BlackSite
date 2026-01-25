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

    [Header("Effects")]
    public GameObject deathEffectPrefab; // Particle effect on death

    [Header("Audio")]
    public AudioClip deathSound; // Reference to death sound
    
    
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

        // Spawn death particle effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            // Create temporary AudioSource for 2D sound
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            
            tempSource.clip = deathSound;
            tempSource.spatialBlend = 0f; // 2D sound (no distance falloff)
            tempSource.volume = 1f; // Full volume
            tempSource.Play();
            
            // Destroy the temporary audio object after sound finishes
            Destroy(tempAudio, deathSound.length);
        }
        
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
