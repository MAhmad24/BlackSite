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
    private float spawnTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        hitEnemies.Clear();
        size = 1f;
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime >= lifetime)
        {
            ReturnToPool();
        }
    }

    public void Initialize(Vector2 direction, int dmg, float spd)
    {
        damage = dmg;
        speed = spd;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
    }

    public void ApplyModifiers(float sizeMultiplier = 1f, float speedMultiplier = 1f, float damageMultiplier = 1f)
    {
        size *= sizeMultiplier;
        speed *= speedMultiplier;
        damage = Mathf.RoundToInt(damage * damageMultiplier);

        transform.localScale *= sizeMultiplier;
        if (rb != null)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null && !target.IsDead)
        {
            if (hitEnemies.Contains(other.gameObject)) return;
            hitEnemies.Add(other.gameObject);
            target.TakeDamage(damage);

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCurrency(CurrencyManager.CURRENCY_PER_HIT, "bullet_hit");
            }
        }
    }

    private void ReturnToPool()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.BulletPool.Return(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
