using UnityEngine;

public class PistolBase : Weapon
{
    [Header("Pistol Settings")]
    public float screenShakePower = 0.1f;
    public float screenShakeDuration = 0.1f;
    
    void Awake()
    {
        // Default pistol stats
        weaponName = "Pistol";
        fireRate = 0.2f;
        bulletSpeed = 20f;
        baseDamage = 1;
        
        SetWeaponStats();
    }
    
    protected virtual void SetWeaponStats()
    {
        // Override in subclasses
    }
    
    // Default pistol firing (single shot) - CAN BE OVERRIDDEN
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        FireSingleShot(direction, firePoint);
        PlayFireSound();
        TriggerScreenShake();
        UpdateFireCooldown();
    }
    
    // Helper method for single shot (reusable)
    protected void FireSingleShot(Vector2 direction, Transform firePoint)
    {
        SpawnBullet(direction, firePoint);
    }
    
    // Helper method for burst fire (for burst pistols)
    protected void FireBurst(Vector2 direction, Transform firePoint, int burstCount, float burstDelay)
    {
        StartCoroutine(BurstFireCoroutine(direction, firePoint, burstCount, burstDelay));
    }
    
    System.Collections.IEnumerator BurstFireCoroutine(Vector2 direction, Transform firePoint, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBullet(direction, firePoint);
            if (i < count - 1) // Don't wait after last bullet
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
    
    protected void PlayFireSound()
    {
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
    }
    
    protected virtual void TriggerScreenShake()
    {
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.TriggerShake(screenShakePower, screenShakeDuration);
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("CONTEXT/PistolBase/Refresh Stats")]
    static void RefreshStats(UnityEditor.MenuCommand command)
    {
        PistolBase weapon = (PistolBase)command.context;
        weapon.SetWeaponStats();
        UnityEditor.EditorUtility.SetDirty(weapon);
    }
    #endif
}