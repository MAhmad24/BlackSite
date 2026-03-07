using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Weapon currentWeapon;
    public Transform firePoint;

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
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying) return;

        if (Input.GetMouseButton(0) && currentWeapon != null && currentWeapon.CanFire())
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            currentWeapon.Fire(direction, firePoint);
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}