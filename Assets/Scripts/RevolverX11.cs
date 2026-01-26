using UnityEngine;

public class RevolverX11 : PistolBase
{
    protected override void SetWeaponStats()
    {
        // Revolver-X11 specific stats
        weaponName = "Revolver-X11";
        fireRate = 0.5f;           // Slower than default pistol
        bulletSpeed = 25f;         // Faster bullets
        baseDamage = 3;            // Much higher damage
        
        // Bigger screen shake for powerful revolver
        screenShakePower = 0.15f;
        screenShakeDuration = 0.15f;
    }
    
    // Optional: Add special effect
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        // Call base pistol fire behavior
        base.Fire(direction, firePoint);
        
        // Add revolver-specific effects here if you want
        // For example: extra muzzle flash, louder sound, etc.
    }
}