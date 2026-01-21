using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 2f; // How long before destroying
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}