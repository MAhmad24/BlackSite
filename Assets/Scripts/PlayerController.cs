using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Camera mainCamera;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Reference to bullet prefab
    public Transform firePoint;     // Where bullets spawn from
    public float fireRate = 0.2f;   // Time between shots
    private float nextFireTime = 0f; // Cooldown tracker
    
    [Header("Camera Shake")]
    public CameraShake cameraShake; // Reference to camera shake script
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
    // Movement input
    float moveX = Input.GetAxisRaw("Horizontal");
    float moveY = Input.GetAxisRaw("Vertical");
    moveInput = new Vector2(moveX, moveY).normalized;
    
    // Shooting input
    HandleShooting();
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    
    void LateUpdate()
    {
        HandleMouseAim();
    }
    
    void HandleMouseAim()
    {
        // Get mouse position in world space
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate direction from player to mouse
        Vector2 direction = mousePos - transform.position;
        
        // Calculate angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Rotate player to face mouse
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void HandleShooting()
{
    // Check if left mouse button is held down AND cooldown is ready
    if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
    {
        Shoot();
        nextFireTime = Time.time + fireRate; // Set next allowed fire time
    }
}

    void Shoot()
    {
        // Get mouse position in world
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate direction to shoot
        Vector2 direction = (mousePos - transform.position).normalized;
        
        // Spawn bullet at player position (or firePoint if we set one)
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        // Initialize bullet with direction
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(direction);

        // Trigger camera shake
        if (cameraShake != null)
        {
            cameraShake.TriggerShake(0.05f, 0.1f); // Small shake: 0.1 power, 0.1 seconds
        }
    }
}