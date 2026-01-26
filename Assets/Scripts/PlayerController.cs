using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Camera mainCamera;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        HandleMovementInput();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    void LateUpdate()
    {
        HandleMouseAim();
    }
    
    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }
    
    void ApplyMovement()
    {
        rb.linearVelocity = moveInput * moveSpeed;
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
}
