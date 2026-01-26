using UnityEngine;

public class PistolBase : Weapon
{
    [Header("Pistol Settings")]
    public float screenShakePower = 0.1f;
    public float screenShakeDuration = 0.1f;
    
    void Awake()
    {
        // Default pistol stats (can be overridden by subclasses)
        weaponName = "Pistol";
        fireRate = 0.2f;
        bulletSpeed = 20f;
        baseDamage = 1;
        
        // Let subclasses override in their Awake if they want
        SetWeaponStats();
    }
    
    // Virtual method that subclasses can override to set their own stats
    protected virtual void SetWeaponStats()
    {
        // Default pistol behavior - subclasses override this
    }
    
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        // All pistols fire a single bullet
        SpawnBullet(direction, firePoint);
        
        // Play sound if available
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
        
        // Trigger screen shake (can be overridden per variant)
        TriggerScreenShake();
        
        // Update cooldown
        UpdateFireCooldown();
    }
    
    // Virtual so variants can have different shake intensities
    protected virtual void TriggerScreenShake()
    {
        // Find camera shake (we'll improve this later with a manager)
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.TriggerShake(screenShakePower, screenShakeDuration);
        }
    }
}