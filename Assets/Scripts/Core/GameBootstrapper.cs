using UnityEngine;

public static class GameBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        EnsureManager<CurrencyManager>("CurrencyManager");
        EnsureManager<GameStateManager>("GameStateManager");
    }

    private static void EnsureManager<T>(string name) where T : MonoBehaviour
    {
        if (Object.FindObjectOfType<T>() == null)
        {
            new GameObject(name).AddComponent<T>();
        }
    }
}
