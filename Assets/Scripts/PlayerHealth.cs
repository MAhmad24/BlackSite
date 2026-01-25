using UnityEngine;
using TMPro; // For TextMeshPro
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    public float invincibilityTime = 1f; // Time after hit before can be hit again

    [Header("UI References")]
    public TextMeshProUGUI healthText; // Reference to UI text
    public GameObject deathScreenUI; // Reference to death screen UI panel
    public TextMeshProUGUI deathKillsText; // Reference to death kills text
    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    [Header("Camera Shake")]
    public CameraShake cameraShake; // Reference to camera shake script
    
    [Header("Audio")]
    public AudioClip damageSound; // Reference to damage/hurt sound
    private AudioSource audioSource; // Reference to AudioSource component
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Player Health: " + currentHealth + "/" + maxHealth);
        UpdateHealthUI();
        audioSource = GetComponent<AudioSource>(); // Get AudioSource
    }
    
    void Update()
    {
        // Handle invincibility timer
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        // Can't take damage if invincible
        if (isInvincible) return;
        
        currentHealth -= damage;
        UpdateHealthUI(); // <-- Add this line
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        // Trigger camera shake
        if (cameraShake != null)
        {
            cameraShake.TriggerShake(0.35f, 0.35f); // Bigger shake: 0.3 power, 0.3 seconds
        }

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        
        // Start invincibility period
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
        
        // Visual feedback (we'll make this flash the sprite)
        StartCoroutine(FlashSprite());
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    System.Collections.IEnumerator FlashSprite()
    {
        // Flash red to show damage
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = sprite.color;
        
        for (int i = 0; i < 3; i++) // Flash 3 times
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        
        // Update death screen with kill count
        if (deathKillsText != null)
        {
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                int kills = waveManager.GetTotalKills();
                deathKillsText.text = "You killed " + kills + " zombies";
            }
        }
        
        // Show death screen UI
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }
    
    // Method to be called by "Play Again" button
    public void RestartGame()
    {
        // Resume time scale
        Time.timeScale = 1f;
        
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    // Method to be called by "Main Menu" button (if you have one)
    public void ReturnToMainMenu()
    {
        // Resume time scale
        Time.timeScale = 1f;
        
        // Load main menu scene (replace "MainMenu" with your actual main menu scene name)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }
}