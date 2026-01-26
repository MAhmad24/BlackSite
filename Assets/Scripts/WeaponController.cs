using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Weapon currentWeapon;     // Current equipped weapon
    public Transform firePoint;       // Where bullets spawn
    
    [Header("Effects")]
    public CameraShake cameraShake;   // Screen shake reference
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        HandleShooting();
    }
    
    void HandleShooting()
    {
        // Check if shooting and weapon is ready
        if (Input.GetMouseButton(0) && currentWeapon != null && currentWeapon.CanFire())
        {
            // Get mouse position and calculate direction
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            
            // Fire the weapon
            currentWeapon.Fire(direction, firePoint);
            
            // Trigger screen shake
            if (cameraShake != null)
            {
                cameraShake.TriggerShake(0.1f, 0.1f);
            }
        }
    }
    
    // Method to switch weapons (for future use)
    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}