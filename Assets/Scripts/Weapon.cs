using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Weapon";
    public float fireRate = 0.2f;      // Time between shots
    public float bulletSpeed = 20f;
    public int baseDamage = 1;
    
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    
    [Header("Audio")]
    public AudioClip fireSound;
    
    protected float nextFireTime = 0f;
    
    // Check if weapon can fire (cooldown ready)
    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }
    
    // Each weapon implements its own fire behavior
    public abstract void Fire(Vector2 direction, Transform firePoint);
    
    // Helper method for subclasses to spawn bullets
    protected GameObject SpawnBullet(Vector2 direction, Transform firePoint)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, baseDamage, bulletSpeed);
        }
        
        return bullet;
    }
    
    // Update cooldown after firing
    protected void UpdateFireCooldown()
    {
        nextFireTime = Time.time + fireRate;
    }
}