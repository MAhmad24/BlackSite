using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string nextScene = "MainMenu";

    void Awake()
    {
        SceneManager.LoadScene(nextScene);
    }
}
