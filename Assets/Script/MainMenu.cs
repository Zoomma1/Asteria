using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    public void StartGame()
    {
        // Load the main game scene
        // TODO: Replace "Game" with the actual name of the game scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        Debug.Log("Start Game");
    }

    public void StartQuizz()
    {
        // Load the quizz game scene
        // TODO: Replace "quizz" with the actual name of the quizz scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("quizz"); 
        Debug.Log("Start Quizz");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
