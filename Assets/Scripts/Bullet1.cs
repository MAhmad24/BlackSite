using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public float lifetime = 3f;
    
    private Rigidbody2D rb;
    
    void Awake()
    {
        // Awake runs BEFORE Start, so rb will be ready when Initialize is called
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Destroy bullet after lifetime expires
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector2 direction)
    {
        // Safety check
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        // Set bullet velocity
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            // Damage the enemy (enemy script handles this)
            // Bullet will be destroyed by enemy script
            return;
        }
        
        // Destroy bullet if it hits anything else (walls, etc.)
        // For now, bullets pass through everything except enemies
    }
}