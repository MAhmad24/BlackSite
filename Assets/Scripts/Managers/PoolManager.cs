using UnityEngine;

/// <summary>
/// Central manager for all object pools. Attach to an empty GameObject in scene.
/// Other scripts access pools via: PoolManager.Instance.BulletPool.Get(...)
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Pool Configuration")]
    public GameObject bulletPrefab;
    public int bulletPoolSize = 50;

    public GameObject enemyPrefab;
    public int enemyPoolSize = 30;

    public ObjectPool BulletPool { get; private set; }
    public ObjectPool EnemyPool { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Transform bulletParent = new GameObject("--- Bullet Pool ---").transform;
        Transform enemyParent = new GameObject("--- Enemy Pool ---").transform;

        BulletPool = new ObjectPool(bulletPrefab, bulletPoolSize, bulletParent);
        EnemyPool = new ObjectPool(enemyPrefab, enemyPoolSize, enemyParent);
    }
}
