using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public void QuitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
