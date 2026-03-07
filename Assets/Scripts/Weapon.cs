using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public WeaponData weaponData;
    public GameObject bulletPrefab;

    private float nextFireTime = 0f;

    public string WeaponName => weaponData != null ? weaponData.weaponName : "Unknown";
    public int BaseDamage => weaponData != null ? weaponData.baseDamage : 1;
    public float FireRate => weaponData != null ? weaponData.fireRate : 0.2f;

    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    public void Fire(Vector2 direction, Transform firePoint)
    {
        if (weaponData == null)
        {
            Debug.LogWarning("Weapon has no WeaponData assigned!");
            return;
        }

        switch (weaponData.fireMode)
        {
            case WeaponFireMode.Single:
            case WeaponFireMode.Auto:
                FireSingle(direction, firePoint);
                break;

            case WeaponFireMode.Burst:
                StartCoroutine(FireBurst(direction, firePoint));
                break;

            case WeaponFireMode.Spread:
                FireSpread(direction, firePoint);
                break;
        }

        PlayFireSound();
        TriggerScreenShake();
        nextFireTime = Time.time + weaponData.fireRate;
    }

    private void FireSingle(Vector2 direction, Transform firePoint)
    {
        SpawnBullet(direction, firePoint);
    }

    private IEnumerator FireBurst(Vector2 direction, Transform firePoint)
    {
        for (int i = 0; i < weaponData.burstCount; i++)
        {
            SpawnBullet(direction, firePoint);
            if (i < weaponData.burstCount - 1)
            {
                yield return new WaitForSeconds(weaponData.burstDelay);
            }
        }
    }

    private void FireSpread(Vector2 direction, Transform firePoint)
    {
        float totalSpread = weaponData.spreadAngle;
        float angleStep = totalSpread / (weaponData.pelletCount - 1);
        float startAngle = -totalSpread / 2f;

        for (int i = 0; i < weaponData.pelletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector2 spreadDirection = RotateVector(direction, currentAngle);
            SpawnBullet(spreadDirection, firePoint);
        }
    }

    private GameObject SpawnBullet(Vector2 direction, Transform firePoint)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject bulletObj;
        if (PoolManager.Instance != null)
        {
            bulletObj = PoolManager.Instance.BulletPool.Get(spawnPos, Quaternion.identity);
        }
        else
        {
            bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        }

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, weaponData.baseDamage, weaponData.bulletSpeed);

            if (weaponData.bulletSizeMultiplier != 1f)
            {
                bullet.ApplyModifiers(sizeMultiplier: weaponData.bulletSizeMultiplier);
            }
        }

        return bulletObj;
    }

    private void PlayFireSound()
    {
        if (weaponData.fireSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
        }
    }

    private void TriggerScreenShake()
    {
        if (Camera.main == null) return;
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.TriggerShake(weaponData.screenShakePower, weaponData.screenShakeDuration);
        }
    }

    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
}
