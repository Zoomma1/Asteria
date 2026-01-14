using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject menuCanvas;
    
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
            
            Debug.Log($"Menu toggled: {isMenuActive}");
        }
    }
}