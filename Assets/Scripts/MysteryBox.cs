using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MysteryBox : MonoBehaviour
{
    [Header("Weapon Pool")]
    [Tooltip("Add weapon prefabs that this box can give")]
    public List<GameObject> weaponPool = new List<GameObject>();
    
    [Header("Box Settings")]
    public float spinDuration = 3f;           // How long to spin before selecting weapon
    public float spinSpeed = 0.1f;            // How fast names cycle during spin
    public int cost = 950;                     // Currency cost (for later)
    // public int cost = 950;                  // UNCOMMENT when currency system ready
    
    [Header("Cooldown")]
    public float cooldownTime = 5f;           // Time before box can be used again
    public bool canUse = true;                // Is box ready to use?
    
    [Header("UI References")]
    public TextMeshProUGUI promptText;        // "Press E to use Mystery Box"
    public Canvas boxCanvas;                  // Canvas for UI above box
    
    [Header("Visual Feedback")]
    public GameObject spinEffect;             // Optional particle effect during spin
    
    private bool playerInRange = false;
    private bool isSpinning = false;
    private bool weaponReady = false;         // Is weapon waiting to be taken/declined?
    private GameObject selectedWeapon;        // Current weapon offered
    
    void Start()
    {
        // Hide UI initially
        if (boxCanvas != null)
        {
            boxCanvas.enabled = false;
        }
        
        UpdatePromptText("");
    }
    
    void Update()
    {
        if (!playerInRange) return;
        
        // State 1: Box ready, player can activate
        if (canUse && !isSpinning && !weaponReady && Input.GetKeyDown(KeyCode.E))
        {
            // CURRENCY CHECK (commented out for now)
            // if (CurrencyManager.Instance.GetCurrency() >= cost)
            // {
            //     CurrencyManager.Instance.SpendCurrency(cost, "mystery_box");
                StartCoroutine(SpinForWeapon());
            // }
            // else
            // {
            //     UpdatePromptText("Not enough currency!");
            // }
        }
        
        // State 2: Weapon ready, player chooses
        if (weaponReady)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TakeWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                DeclineWeapon();
            }
        }
    }
    
    IEnumerator SpinForWeapon()
    {
        isSpinning = true;
        UpdatePromptText("Mystery Box spinning...");
        
        // Start spin effect if available
        if (spinEffect != null)
        {
            spinEffect.SetActive(true);
        }
        
        // Cycle through weapon names rapidly
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            // Pick random weapon from pool to display
            GameObject randomWeapon = weaponPool[Random.Range(0, weaponPool.Count)];
            Weapon weaponScript = randomWeapon.GetComponent<Weapon>();
            
            if (weaponScript != null)
            {
                UpdatePromptText(weaponScript.weaponName);
            }
            
            yield return new WaitForSeconds(spinSpeed);
            elapsed += spinSpeed;
        }
        
        // Stop spin effect
        if (spinEffect != null)
        {
            spinEffect.SetActive(false);
        }
        
        // Select final weapon
        selectedWeapon = weaponPool[Random.Range(0, weaponPool.Count)];
        Weapon finalWeapon = selectedWeapon.GetComponent<Weapon>();
        
        if (finalWeapon != null)
        {
            UpdatePromptText($"{finalWeapon.weaponName}\nPress E to take | Press Q to decline");
        }
        
        isSpinning = false;
        weaponReady = true;
    }
    
    void TakeWeapon()
    {
        Debug.Log("Player took weapon: " + selectedWeapon.name);
        
        // Find player's weapon controller
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                // Destroy old weapon if exists
                if (weaponController.currentWeapon != null)
                {
                    Destroy(weaponController.currentWeapon.gameObject);
                }
                
                // Instantiate new weapon as child of player
                GameObject newWeapon = Instantiate(selectedWeapon, player.transform);
                Weapon weaponScript = newWeapon.GetComponent<Weapon>();
                
                // Equip the new weapon
                weaponController.EquipWeapon(weaponScript);
                
                UpdatePromptText($"Equipped {weaponScript.weaponName}!");
            }
        }
        
        // Reset box
        weaponReady = false;
        StartCoroutine(CooldownTimer());
    }
    
    void DeclineWeapon()
    {
        Debug.Log("Player declined weapon: " + selectedWeapon.name);
        
        UpdatePromptText("Weapon declined");
        
        // Reset box
        weaponReady = false;
        StartCoroutine(CooldownTimer());
    }
    
    IEnumerator CooldownTimer()
    {
        canUse = false;
        
        float remaining = cooldownTime;
        while (remaining > 0)
        {
            UpdatePromptText($"Cooldown: {remaining:F1}s");
            yield return new WaitForSeconds(0.1f);
            remaining -= 0.1f;
        }
        
        canUse = true;
        UpdatePromptText("Press E to use Mystery Box");
    }
    
    void UpdatePromptText(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            if (boxCanvas != null)
            {
                boxCanvas.enabled = true;
            }
            
            if (canUse && !weaponReady)
            {
                // UNCOMMENT when currency system ready:
                // UpdatePromptText($"Press E to use Mystery Box (Cost: {cost})");
                UpdatePromptText("Press E to use Mystery Box");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (boxCanvas != null)
            {
                boxCanvas.enabled = false;
            }
        }
    }
}
