using UnityEngine;
using System.Collections.Generic;
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public float lifetime = 3f;
    public int damage = 1;
    
    private Rigidbody2D rb;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
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
    if (other.CompareTag("Enemy"))
    {
        // Check if we already hit this enemy
        if (hitEnemies.Contains(other.gameObject))
        {
            return; // Already damaged this one, skip
        }
        
        // Mark as hit
        hitEnemies.Add(other.gameObject);
        
        // Damage the enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
    
    // For now, bullets pass through everything except enemies
}
}