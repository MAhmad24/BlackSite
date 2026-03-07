using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponName = "New Weapon";
    public Sprite weaponIcon;
    [TextArea] public string description;

    [Header("Firing Stats")]
    public float fireRate = 0.2f;
    public float bulletSpeed = 20f;
    public int baseDamage = 1;

    [Header("Bullet Modifiers")]
    public float bulletSizeMultiplier = 1f;

    [Header("Behavior Type")]
    public WeaponFireMode fireMode = WeaponFireMode.Single;
    public int burstCount = 3;
    public float burstDelay = 0.05f;
    public int pelletCount = 7;
    public float spreadAngle = 20f;

    [Header("Effects")]
    public float screenShakePower = 0.1f;
    public float screenShakeDuration = 0.1f;
    public AudioClip fireSound;

    [Header("Economy")]
    public int purchaseCost = 0;
    public int mysteryBoxWeight = 1;
}

public enum WeaponFireMode
{
    Single,
    Burst,
    Spread,
    Auto
}
