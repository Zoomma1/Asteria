using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized VR Controller Input Manager (Singleton)
/// Manages all Meta Quest controller inputs in one place for easy reusability.
/// Place this script on the VR Player GameObject.
/// </summary>
public class VRControllerInputs : MonoBehaviour
{
    public static VRControllerInputs Instance { get; private set; }
    
    [Header("Activation Thresholds")]
    [SerializeField] 
    [Range(0f, 1f)]
    [Tooltip("Value above which trigger/grip is considered pressed")]
    private float buttonThreshold = 0.5f;
    
    // Input Actions
    private InputAction leftTriggerAction;
    private InputAction rightTriggerAction;
    private InputAction leftGripAction;
    private InputAction rightGripAction;
    private InputAction leftPrimaryButtonAction;
    private InputAction rightPrimaryButtonAction;
    private InputAction leftSecondaryButtonAction;
    private InputAction rightSecondaryButtonAction;
    
    // Public properties for easy access
    
    #region Trigger
    /// <summary>Left trigger value (0-1)</summary>
    public float LeftTriggerValue { get; private set; }
    
    /// <summary>Right trigger value (0-1)</summary>
    public float RightTriggerValue { get; private set; }
    
    /// <summary>Is left trigger pressed above threshold?</summary>
    public bool IsLeftTriggerPressed => LeftTriggerValue > buttonThreshold;
    
    /// <summary>Is right trigger pressed above threshold?</summary>
    public bool IsRightTriggerPressed => RightTriggerValue > buttonThreshold;
    #endregion
    
    #region Grip
    /// <summary>Left grip value (0-1)</summary>
    public float LeftGripValue { get; private set; }
    
    /// <summary>Right grip value (0-1)</summary>
    public float RightGripValue { get; private set; }
    
    /// <summary>Is left grip pressed above threshold?</summary>
    public bool IsLeftGripPressed => LeftGripValue > buttonThreshold;
    
    /// <summary>Is right grip pressed above threshold?</summary>
    public bool IsRightGripPressed => RightGripValue > buttonThreshold;
    #endregion
    
    #region Primary Button (A/X)
    /// <summary>Is left primary button (X) pressed?</summary>
    public bool IsLeftPrimaryButtonPressed { get; private set; }
    
    /// <summary>Is right primary button (A) pressed?</summary>
    public bool IsRightPrimaryButtonPressed { get; private set; }
    
    /// <summary>Was left primary button pressed this frame?</summary>
    public bool LeftPrimaryButtonDown { get; private set; }
    
    /// <summary>Was right primary button pressed this frame?</summary>
    public bool RightPrimaryButtonDown { get; private set; }
    #endregion
    
    #region Secondary Button (B/Y)
    /// <summary>Is left secondary button (Y) pressed?</summary>
    public bool IsLeftSecondaryButtonPressed { get; private set; }
    
    /// <summary>Is right secondary button (B) pressed?</summary>
    public bool IsRightSecondaryButtonPressed { get; private set; }
    
    /// <summary>Was left secondary button pressed this frame?</summary>
    public bool LeftSecondaryButtonDown { get; private set; }
    
    /// <summary>Was right secondary button pressed this frame?</summary>
    public bool RightSecondaryButtonDown { get; private set; }
    #endregion
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("[VRControllerInputs] Duplicate instance destroyed!");
            return;
        }
        Instance = this;
        
        InitializeInputActions();
        Debug.Log("[VRControllerInputs] Initialized successfully");
    }
    
    private void InitializeInputActions()
    {
        // Left Trigger
        leftTriggerAction = new InputAction(
            name: "Left Trigger",
            type: InputActionType.Value,
            binding: "<XRController>{LeftHand}/trigger"
        );
        
        // Right Trigger
        rightTriggerAction = new InputAction(
            name: "Right Trigger",
            type: InputActionType.Value,
            binding: "<XRController>{RightHand}/trigger"
        );
        
        // Left Grip
        leftGripAction = new InputAction(
            name: "Left Grip",
            type: InputActionType.Value,
            binding: "<XRController>{LeftHand}/grip"
        );
        
        // Right Grip
        rightGripAction = new InputAction(
            name: "Right Grip",
            type: InputActionType.Value,
            binding: "<XRController>{RightHand}/grip"
        );
        
        // Left Primary Button (X)
        leftPrimaryButtonAction = new InputAction(
            name: "Left Primary Button",
            type: InputActionType.Button,
            binding: "<XRController>{LeftHand}/primaryButton"
        );
        
        // Right Primary Button (A)
        rightPrimaryButtonAction = new InputAction(
            name: "Right Primary Button",
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/primaryButton"
        );
        
        // Left Secondary Button (Y)
        leftSecondaryButtonAction = new InputAction(
            name: "Left Secondary Button",
            type: InputActionType.Button,
            binding: "<XRController>{LeftHand}/secondaryButton"
        );
        
        // Right Secondary Button (B)
        rightSecondaryButtonAction = new InputAction(
            name: "Right Secondary Button",
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/secondaryButton"
        );
    }
    
    private void OnEnable()
    {
        leftTriggerAction?.Enable();
        rightTriggerAction?.Enable();
        leftGripAction?.Enable();
        rightGripAction?.Enable();
        leftPrimaryButtonAction?.Enable();
        rightPrimaryButtonAction?.Enable();
        leftSecondaryButtonAction?.Enable();
        rightSecondaryButtonAction?.Enable();
    }
    
    private void OnDisable()
    {
        leftTriggerAction?.Disable();
        rightTriggerAction?.Disable();
        leftGripAction?.Disable();
        rightGripAction?.Disable();
        leftPrimaryButtonAction?.Disable();
        rightPrimaryButtonAction?.Disable();
        leftSecondaryButtonAction?.Disable();
        rightSecondaryButtonAction?.Disable();
    }
    
    private void Update()
    {
        // Read trigger values
        LeftTriggerValue = leftTriggerAction?.ReadValue<float>() ?? 0f;
        RightTriggerValue = rightTriggerAction?.ReadValue<float>() ?? 0f;
        
        // Read grip values
        LeftGripValue = leftGripAction?.ReadValue<float>() ?? 0f;
        RightGripValue = rightGripAction?.ReadValue<float>() ?? 0f;
        
        // Read primary button states
        IsLeftPrimaryButtonPressed = leftPrimaryButtonAction?.IsPressed() ?? false;
        IsRightPrimaryButtonPressed = rightPrimaryButtonAction?.IsPressed() ?? false;
        LeftPrimaryButtonDown = leftPrimaryButtonAction?.WasPressedThisFrame() ?? false;
        RightPrimaryButtonDown = rightPrimaryButtonAction?.WasPressedThisFrame() ?? false;
        
        // Read secondary button states
        IsLeftSecondaryButtonPressed = leftSecondaryButtonAction?.IsPressed() ?? false;
        IsRightSecondaryButtonPressed = rightSecondaryButtonAction?.IsPressed() ?? false;
        LeftSecondaryButtonDown = leftSecondaryButtonAction?.WasPressedThisFrame() ?? false;
        RightSecondaryButtonDown = rightSecondaryButtonAction?.WasPressedThisFrame() ?? false;
    }
    
    private void OnDestroy()
    {
        leftTriggerAction?.Dispose();
        rightTriggerAction?.Dispose();
        leftGripAction?.Dispose();
        rightGripAction?.Dispose();
        leftPrimaryButtonAction?.Dispose();
        rightPrimaryButtonAction?.Dispose();
        leftSecondaryButtonAction?.Dispose();
        rightSecondaryButtonAction?.Dispose();
        
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    /// <summary>
    /// Set the threshold for trigger/grip to be considered pressed
    /// </summary>
    public void SetButtonThreshold(float threshold)
    {
        buttonThreshold = Mathf.Clamp01(threshold);
    }
    
    /// <summary>
    /// Get current threshold value
    /// </summary>
    public float GetButtonThreshold()
    {
        return buttonThreshold;
    }
}

