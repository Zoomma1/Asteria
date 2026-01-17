using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject menuCanvas;
    [Tooltip("Alternative canvas - will be disabled when the main menu is active")]
    public GameObject alternativeCanvas;
    
    [Header("Input Settings")]
    [Tooltip("Which hand to use for menu toggle: LeftHand or RightHand")]
    public string handUsage = "RightHand";

    void Update()
    {
        if (VRControllerInputs.Instance == null) return;
        
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
            
            // Disable the alternative canvas when the menu is activated
            if (alternativeCanvas != null)
            {
                alternativeCanvas.SetActive(!isMenuActive);
                Debug.Log($"Alternative canvas toggled: {!isMenuActive}");
            }
            
            Debug.Log($"Menu toggled: {isMenuActive}");
        }
    }

    // Enables the menu canvas and disables the alternative canvas
    public void SwitchToMenuCanvas()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
            Debug.Log($"Menu canvas enabled");
        }

        if (alternativeCanvas != null)
        {
            alternativeCanvas.SetActive(false);
            Debug.Log($"Alternative canvas disabled");
        }
    }

    // Enables the alternative canvas and disables the menu canvas
    public void SwitchToAlternativeCanvas()
    {
        if (alternativeCanvas != null)
        {
            alternativeCanvas.SetActive(true);
            Debug.Log($"Alternative canvas enabled");
        }

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
            Debug.Log($"Menu canvas disabled");
        }
    }

    // Toggles between the two canvases exclusively
    public void ToggleBetweenCanvases()
    {
        if (menuCanvas != null && alternativeCanvas != null)
        {
            bool menuIsActive = menuCanvas.activeSelf;
            
            menuCanvas.SetActive(!menuIsActive);
            alternativeCanvas.SetActive(menuIsActive);
            
            Debug.Log($"Menu canvas is now {(!menuIsActive ? "enabled" : "disabled")}");
            Debug.Log($"Alternative canvas is now {(menuIsActive ? "enabled" : "disabled")}");
        }
        else
        {
            Debug.LogWarning("Both canvases must be assigned for toggle functionality!");
        }
    }
}