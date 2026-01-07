using UnityEngine;
using UnityEngine.SceneManagement; // Obligatoire pour charger des scènes

namespace Menu
{
public class MenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";

    // Cette fonction sera appelée par le bouton avec un paramètre string
    // Dans l'inspecteur, vous pouvez entrer le nom de la scène dans le champ "String Argument"
    public void ChargerScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);  
        SceneManager.LoadScene(sceneName);
    }

    // Fonction sans paramètre pour charger la scène définie dans l'inspecteur
    // Plus facile à utiliser dans l'inspecteur Unity
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

    // Optionnel : pour quitter le jeu
    public void GameQuit()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}
}