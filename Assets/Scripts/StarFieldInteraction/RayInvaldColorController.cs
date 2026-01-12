using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace StarFieldInteraction
{
    /// <summary>
    /// Controls the visibility of the invalid ray color gradient.
    /// By default, ray appears when right trigger is pressed (via VRControllerInputs).
    /// Can be overridden with manual control from Inspector for debugging.
    /// Visible ray: white with fade effect. Hidden ray: transparent.
    /// </summary>
    public class RayColorController : MonoBehaviour
    {
    [Header("References")]
    [SerializeField] private XRInteractorLineVisual lineVisual;
    
    [Header("Visibility Settings")]
    [SerializeField] 
    [Tooltip("Override trigger input to force visibility. When enabled, uses manual control instead of trigger")]
    private bool overrideWithManualControl = false;
    
    [SerializeField] 
    [Tooltip("Manual visibility control (only used when Override is enabled)")]
    private bool manualVisibility = false;
        
        private Gradient transparentGradient;
        private Gradient visibleGradient;
        
        private void Awake()
        {
            // Get component if not assigned
            if (lineVisual == null)
            {
                lineVisual = GetComponent<XRInteractorLineVisual>();
            }
            
            // Create transparent gradient (invisible)
            transparentGradient = new Gradient();
            GradientColorKey[] transparentColorKeys = new GradientColorKey[2];
            transparentColorKeys[0] = new GradientColorKey(Color.white, 0f);
            transparentColorKeys[1] = new GradientColorKey(Color.white, 1f);
            
            GradientAlphaKey[] transparentAlphaKeys = new GradientAlphaKey[2];
            transparentAlphaKeys[0] = new GradientAlphaKey(0f, 0f); // Fully transparent
            transparentAlphaKeys[1] = new GradientAlphaKey(0f, 1f); // Fully transparent
            
            transparentGradient.SetKeys(transparentColorKeys, transparentAlphaKeys);
            
            // Create visible white gradient with fade effect
            visibleGradient = new Gradient();
            GradientColorKey[] visibleColorKeys = new GradientColorKey[2];
            visibleColorKeys[0] = new GradientColorKey(Color.white, 0f);
            visibleColorKeys[1] = new GradientColorKey(Color.white, 1f);
            
            GradientAlphaKey[] visibleAlphaKeys = new GradientAlphaKey[2];
            visibleAlphaKeys[0] = new GradientAlphaKey(1f, 0f); // Fully opaque at start
            visibleAlphaKeys[1] = new GradientAlphaKey(0f, 1f); // Fully transparent at end (fade effect)
            
            visibleGradient.SetKeys(visibleColorKeys, visibleAlphaKeys);
            
            Debug.Log("[RayColorController] Gradients initialized with fade effect");
        }
        
    private void Update()
    {
        if (lineVisual == null) return;
        
        // Determine if ray should be visible
        bool shouldShowRay;
        
        if (overrideWithManualControl)
        {
            // Use manual control from Inspector
            shouldShowRay = manualVisibility;
        }
        else
        {
            // Use right trigger input
            if (VRControllerInputs.Instance != null)
            {
                shouldShowRay = VRControllerInputs.Instance.IsRightTriggerPressed;
            }
            else
            {
                Debug.LogWarning("[RayColorController] VRControllerInputs instance not found!");
                shouldShowRay = false;
            }
        }
        
        // Apply gradient based on visibility state
        if (shouldShowRay)
        {
            lineVisual.invalidColorGradient = visibleGradient;
        }
        else
        {
            lineVisual.invalidColorGradient = transparentGradient;
        }
    }
        
        private void OnValidate()
        {
            // Auto-assign component in editor
            if (lineVisual == null)
            {
                lineVisual = GetComponent<XRInteractorLineVisual>();
            }
        }
        
    /// <summary>
    /// Programmatically set the invalid ray visibility (enables manual override)
    /// </summary>
    public void SetInvalidRayVisible(bool visible)
    {
        overrideWithManualControl = true;
        manualVisibility = visible;
    }
    
    /// <summary>
    /// Get current visibility state
    /// </summary>
    public bool IsInvalidRayVisible()
    {
        if (overrideWithManualControl)
        {
            return manualVisibility;
        }
        else if (VRControllerInputs.Instance != null)
        {
            return VRControllerInputs.Instance.IsRightTriggerPressed;
        }
        return false;
    }
    
    /// <summary>
    /// Disable manual override and use trigger input
    /// </summary>
    public void UseAutomaticTriggerControl()
    {
        overrideWithManualControl = false;
    }
    }
}