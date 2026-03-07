using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MysteryBox : MonoBehaviour
{
    [Header("Weapon Pool")]
    public List<WeaponData> weaponPool = new List<WeaponData>();

    [Header("Weapon Prefab")]
    public GameObject weaponPrefab;

    [Header("Box Settings")]
    public float spinDuration = 3f;
    public float spinSpeed = 0.1f;
    public int cost = 950;
    public float cooldownTime = 5f;
    public bool canUse = true;

    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public Canvas boxCanvas;

    [Header("Visual Feedback")]
    public GameObject spinEffect;

    private bool playerInRange = false;
    private bool isSpinning = false;
    private bool weaponReady = false;
    private WeaponData selectedWeaponData;

    void Start()
    {
        if (boxCanvas != null) boxCanvas.enabled = false;
        UpdatePromptText("");
    }

    void Update()
    {
        if (!playerInRange) return;

        if (canUse && !isSpinning && !weaponReady && Input.GetKeyDown(KeyCode.E))
        {
            if (CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(cost))
            {
                CurrencyManager.Instance.SpendCurrency(cost, "mystery_box");
                StartCoroutine(SpinForWeapon());
            }
            else if (CurrencyManager.Instance != null)
            {
                UpdatePromptText("Not enough currency!");
            }
            else
            {
                // No currency system yet — allow free use
                StartCoroutine(SpinForWeapon());
            }
        }

        if (weaponReady)
        {
            if (Input.GetKeyDown(KeyCode.E)) TakeWeapon();
            else if (Input.GetKeyDown(KeyCode.Q)) DeclineWeapon();
        }
    }

    IEnumerator SpinForWeapon()
    {
        isSpinning = true;
        UpdatePromptText("Mystery Box spinning...");

        if (spinEffect != null) spinEffect.SetActive(true);

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            WeaponData randomWeapon = weaponPool[Random.Range(0, weaponPool.Count)];
            UpdatePromptText(randomWeapon.weaponName);
            yield return new WaitForSeconds(spinSpeed);
            elapsed += spinSpeed;
        }

        if (spinEffect != null) spinEffect.SetActive(false);

        selectedWeaponData = weaponPool[Random.Range(0, weaponPool.Count)];
        UpdatePromptText($"{selectedWeaponData.weaponName}\nPress E to take | Press Q to decline");

        isSpinning = false;
        weaponReady = true;
    }

    void TakeWeapon()
    {
        Debug.Log("Player took weapon: " + selectedWeaponData.weaponName);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                if (weaponController.currentWeapon != null)
                {
                    Destroy(weaponController.currentWeapon.gameObject);
                }

                GameObject newWeaponObj = Instantiate(weaponPrefab, player.transform);
                Weapon weaponScript = newWeaponObj.GetComponent<Weapon>();
                weaponScript.weaponData = selectedWeaponData;

                weaponController.EquipWeapon(weaponScript);
                UpdatePromptText($"Equipped {selectedWeaponData.weaponName}!");
            }
        }

        weaponReady = false;
        StartCoroutine(CooldownTimer());
    }

    void DeclineWeapon()
    {
        UpdatePromptText("Weapon declined");
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
        if (playerInRange) UpdatePromptText($"Press E to use Mystery Box (Cost: ${cost})");
    }

    void UpdatePromptText(string message)
    {
        if (promptText != null) promptText.text = message;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (boxCanvas != null) boxCanvas.enabled = true;
            if (canUse && !weaponReady) UpdatePromptText($"Press E to use Mystery Box (Cost: ${cost})");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (boxCanvas != null) boxCanvas.enabled = false;
        }
    }
}
