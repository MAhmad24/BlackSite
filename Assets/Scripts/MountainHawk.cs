using UnityEngine;

public class MountainHawk : PistolBase
{
    [Header("Mountain Hawk Special")]
    public float bulletSizeMultiplier = 1.5f; // Bigger bullets!
    
    protected override void SetWeaponStats()
    {
        weaponName = "Mountain Hawk";
        fireRate = 0.8f;           // Very slow
        bulletSpeed = 30f;         // Very fast
        baseDamage = 5;            // Massive damage
        
        // Huge screen shake
        screenShakePower = 0.3f;
        screenShakeDuration = 0.2f;
    }
    
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        // Fire the bullet
        GameObject bulletObj = SpawnBullet(direction, firePoint);
        
        // Make the bullet bigger (special Desert Eagle trait!)
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ApplyModifiers(sizeMultiplier: bulletSizeMultiplier);
        }
        
        // Play sound
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
        
        // Big screen shake
        TriggerScreenShake();
        
        UpdateFireCooldown();
    }
}