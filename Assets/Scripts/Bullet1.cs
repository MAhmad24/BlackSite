using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public float speed = 20f;
    public float size = 1f;
    public int damage = 1;
    public float lifetime = 3f;
    
    private Rigidbody2D rb;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Apply visual properties
        transform.localScale *= size;
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    // Initialize bullet with direction, damage, and speed
    public void Initialize(Vector2 direction, int dmg, float spd)
    {
        damage = dmg;
        speed = spd;
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        rb.linearVelocity = direction * speed;
    }
    
    // Apply upgrade modifiers (for future powerups)
    public void ApplyModifiers(float sizeMultiplier = 1f, float speedMultiplier = 1f, float damageMultiplier = 1f)
    {
        size *= sizeMultiplier;
        speed *= speedMultiplier;
        damage = Mathf.RoundToInt(damage * damageMultiplier);
        
        // Apply new values (multiply, don't replace)
        transform.localScale *= sizeMultiplier;  // <-- Changed to just apply the multiplier directly
        if (rb != null)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check if we already hit this enemy
            if (hitEnemies.Contains(other.gameObject))
            {
                return;
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
    }
}