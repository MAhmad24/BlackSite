using UnityEngine;

public class Glock9000 : PistolBase
{
    protected override void SetWeaponStats()
    {
        // Glock-9000: Fast firing, low damage
        weaponName = "Glock-9000";
        fireRate = 0.1f;           // Very fast
        bulletSpeed = 22f;
        baseDamage = 1;            // Low damage per bullet
        
        // Minimal screen shake for controlled weapon
        screenShakePower = 0.05f;
        screenShakeDuration = 0.05f;
    }
}