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
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            if (hitEnemies.Contains(other.gameObject)){
                return;
            }

            hitEnemies.add(other.gameObject);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null){
                enemy.TakeDamage(damage);
            }
        }
        
        // Destroy bullet if it hits anything else (walls, etc.)
        // For now, bullets pass through everything except enemies
    }
}