using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    public void StartGame()
    {
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        // Debug.Log("Start Game");
    }

    public void StartQuizz()
    {
        // Load the quizz game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuizzScene"); 
        // Debug.Log("Quizz Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
