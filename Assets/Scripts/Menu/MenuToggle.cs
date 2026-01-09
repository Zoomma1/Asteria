using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas; // Glisse ton Canvas ici
    public InputActionProperty buttonA; // L'input du bouton A

    void Update()
    {
        // Vérifie si le bouton A vient d'être pressé
        if (buttonA.action.WasPressedThisFrame())
        {
            // Inverse l'état actuel du Canvas (Affiche s'il est caché, Cache s'il est affiché)
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}