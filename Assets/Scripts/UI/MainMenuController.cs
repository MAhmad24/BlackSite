using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameScene = "SampleScene";

    public void StartRun()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetState(GameState.Playing);
        else
            Time.timeScale = 1f;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.StartNewSession();

        SceneManager.LoadScene(gameScene);
    }
}
