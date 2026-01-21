using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private float shakeTimeRemaining = 0f;
    private float shakePower = 0f;
    private float shakeFadeTime = 0f;
    
    void Start()
    {
        // Store the camera's starting position
        originalPosition = transform.localPosition;
    }
    
    void LateUpdate()
    {
        // If we're shaking
        if (shakeTimeRemaining > 0)
        {
            // Calculate how much shake is left (0 to 1)
            float shakeAmount = shakePower * (shakeTimeRemaining / shakeFadeTime);
            
            // Apply random offset to camera
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            
            // Reduce shake time
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            // Shake finished, return to original position
            shakeTimeRemaining = 0f;
            transform.localPosition = originalPosition;
        }
    }
    
    // Call this function to trigger a shake
    public void TriggerShake(float power, float duration)
    {
        shakeTimeRemaining = duration;
        shakePower = power;
        shakeFadeTime = duration;
    }
}