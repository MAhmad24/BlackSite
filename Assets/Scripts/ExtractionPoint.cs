using UnityEngine;
using System.Collections;

public class ExtractionPoint : MonoBehaviour
{
    [Header("Extraction Settings")]
    public float activationTime = 3f; // How long to hold E
    public float extractionDuration = 10f; // How long to survive after activation
    
    private bool isPlayerInRange = false;
    private bool isActivated = false;
    private bool isExtracting = false;
    private float activationProgress = 0f;
    
    void Update()
    {
        if (isActivated || isExtracting) return; // Already activated
        
        // Check if player is holding E while in range
        if (isPlayerInRange && Input.GetKey(KeyCode.E))
        {
            activationProgress += Time.deltaTime;
            
            Debug.Log("Activating extraction... " + (activationProgress / activationTime * 100f).ToString("F0") + "%");
            
            if (activationProgress >= activationTime)
            {
                ActivateExtraction();
            }
        }
        else
        {
            // Reset if player lets go
            activationProgress = 0f;
        }
    }
    
    void ActivateExtraction()
    {
        isActivated = true;
        isExtracting = true;
        
        Debug.Log("EXTRACTION ACTIVATED! Survive for " + extractionDuration + " seconds!");
        
        // Notify WaveManager to trigger final wave
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.TriggerExtractionWave();
        }
        
        // Start extraction timer
        StartCoroutine(ExtractionTimer());
    }
    
    IEnumerator ExtractionTimer()
    {
        yield return new WaitForSeconds(extractionDuration);
        
        // Player survived! Extract successfully
        ExtractionSuccess();
    }
    
    void ExtractionSuccess()
    {
        Debug.Log("EXTRACTION SUCCESSFUL!");
        
        // TODO: Save currency, show success screen
        // For now, just reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Extraction point in range. Hold E to activate!");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            activationProgress = 0f;
        }
    }
}