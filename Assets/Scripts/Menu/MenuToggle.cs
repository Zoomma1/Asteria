using UnityEngine;
using UnityEngine.InputSystem; 

public class MenuToggle : MonoBehaviour
{
    public GameObject menuCanvas;      // Glisse ton Canvas ici
    public Transform cameraTransform; // Glisse ta Main Camera ici
    public float distance = 2.0f;     // Distance à laquelle le menu apparaît
    
    // Référence à l'action du bouton (on va la configurer dans l'inspecteur)
    public InputActionProperty menuButtonAction;

    void Update()
    {
        // Vérifie si le bouton a été pressé à cette image
        if (menuButtonAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        // Inverse l'état (si actif -> devient inactif et inversement)
        bool isActive = !menuCanvas.activeSelf;
        menuCanvas.SetActive(isActive);

        if (isActive)
        {
            // Positionne le menu devant la caméra
            Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
            
            // Aligne la hauteur sur les yeux
            targetPosition.y = cameraTransform.position.y;
            
            menuCanvas.transform.position = targetPosition;

            // Fait en sorte que le menu regarde le joueur
            Vector3 lookAtPos = cameraTransform.position;
            lookAtPos.y = menuCanvas.transform.position.y;
            menuCanvas.transform.LookAt(lookAtPos);
            menuCanvas.transform.Rotate(0, 180, 0);
        }
    }
}