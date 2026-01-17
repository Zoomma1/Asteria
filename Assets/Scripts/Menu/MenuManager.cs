using UnityEngine;
using UnityEngine.SceneManagement; // Required to load scenes

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private GameObject canvasToToggle;

    // This function will be called by the button with a string parameter
    // In the inspector, you can enter the scene name in the "String Argument" field
    public void ChargerScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);  
        SceneManager.LoadScene(sceneName);
    }

    // Function without parameter to load the scene defined in the inspector
    // Easier to use in Unity inspector
    public void GameLoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene defined in sceneToLoad !");
        }
    }

    // Enable/Disable the Canvas defined in the inspector
    public void ToggleCanvas()
    {
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(!canvasToToggle.activeSelf);
            Debug.Log($"Canvas {canvasToToggle.name} is now {(canvasToToggle.activeSelf ? "enabled" : "disabled")}");
        }
        else
        {
            Debug.LogWarning("No Canvas assigned to canvasToToggle!");
        }
    }

    // Enable the Canvas defined in the inspector
    public void EnableCanvas()
    {
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(true);
            Debug.Log($"Canvas {canvasToToggle.name} enabled");
        }
        else
        {
            Debug.LogWarning("No Canvas assigned to canvasToToggle!");
        }
    }

    // Disable the Canvas defined in the inspector
    public void DisableCanvas()
    {
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(false);
            Debug.Log($"Canvas {canvasToToggle.name} disabled");
        }
        else
        {
            Debug.LogWarning("No Canvas assigned to canvasToToggle!");
        }
    }

    // Enable/Disable a specific Canvas passed as parameter
    public void ToggleSpecificCanvas(GameObject canvas)
    {
        if (canvas != null)
        {
            canvas.SetActive(!canvas.activeSelf);
            Debug.Log($"Canvas {canvas.name} is now {(canvas.activeSelf ? "enabled" : "disabled")}");
        }
        else
        {
            Debug.LogWarning("Canvas parameter is null!");
        }
    }

    // Optional: to quit the game
    public void GameQuit()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}