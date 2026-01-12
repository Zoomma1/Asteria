using UnityEngine;
using StarFieldInteraction;

public class MenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject menuCanvas;
    
    [Header("Input Settings")]
    [Tooltip("Which hand to use for menu toggle: LeftHand or RightHand")]
    public string handUsage = "RightHand";
    
    [Header("Ray Control")]
    [SerializeField] private RayVisibilityController rayController;

    void Update()
    {
        // Check if VRControllerInputs is available
        if (VRControllerInputs.Instance == null)
        {
            return;
        }
        
        // Check which hand's primary button was pressed
        bool primaryButtonPressed = false;
        
        if (handUsage == "RightHand")
        {
            primaryButtonPressed = VRControllerInputs.Instance.RightPrimaryButtonDown;
        }
        else if (handUsage == "LeftHand")
        {
            primaryButtonPressed = VRControllerInputs.Instance.LeftPrimaryButtonDown;
        }
        
        if (primaryButtonPressed)
        {
            bool isMenuActive = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isMenuActive);
            
            // Disable ray when menu is open, enable when menu is closed
            if (rayController != null)
            {
                rayController.SetRayEnabled(!isMenuActive);
            }
            
            Debug.Log($"Menu toggled: {isMenuActive}");
        }
    }
}