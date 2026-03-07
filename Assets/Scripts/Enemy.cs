using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
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
    public GameObject deathEffectPrefab;

    [Header("Audio")]
    public AudioClip deathSound;

    public bool IsDead => isDead;

    void OnEnable()
    {
        isDead = false;
    }

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
        if (isDead) return;

        health -= damageAmount;
        Debug.Log("Enemy hit! Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("=== ENEMY DIE() CALLED ===");

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(CurrencyManager.CURRENCY_PER_KILL, "kill_basic");
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

            tempSource.clip = deathSound;
            tempSource.spatialBlend = 0f;
            tempSource.volume = 1f;
            tempSource.Play();

            Destroy(tempAudio, deathSound.length);
        }

        // Disable colliders immediately so bullets stop hitting
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        rb.linearVelocity = Vector2.zero;

        if (OnDeath != null)
        {
            OnDeath.Invoke();
            OnDeath = null;
        }

        Invoke("ReturnToPool", 0.1f);
    }

    private void ReturnToPool()
    {
        isDead = false;
        health = 3;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = true;
        }

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.EnemyPool.Return(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
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
