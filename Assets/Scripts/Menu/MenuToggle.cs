using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    [Tooltip("The canvas to show/hide when the button is pressed")]
    public GameObject menuCanvas;
    
    [Header("Input Settings")]
    [Tooltip("Which hand's primary button to use (RightHand or LeftHand)")]
    public string handUsage = "RightHand"; // Can be "RightHand" or "LeftHand"
    
    private InputAction menuToggleAction;

    void OnEnable()
    {
        // Create a direct input action for the primary button of the specified XR Controller
        menuToggleAction = new InputAction(
            name: "Menu Toggle",
            type: InputActionType.Button,
            binding: $"<XRController>{{{handUsage}}}/primaryButton"
        );
        menuToggleAction.Enable();
    }

    void OnDisable()
    {
        // Clean up the input action
        menuToggleAction?.Disable();
        menuToggleAction?.Dispose();
    }

    void Update()
    {
        // Check if the primary button was pressed this frame
        if (menuToggleAction != null && menuToggleAction.WasPressedThisFrame())
        {
            // Toggle the menu canvas visibility
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            Debug.Log($"Menu toggled: {menuCanvas.activeSelf}");
        }
    }
}