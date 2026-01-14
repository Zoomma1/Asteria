using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

namespace StarFieldInteraction
{
    /// <summary>
    /// Toggles the XR Ray Interactor based on the controller's grip button state
    /// Blocks star interaction when trigger is not pressed
    /// Attach this script to the GameObject containing the XRRayInteractor component
    /// </summary>
    [RequireComponent(typeof(XRRayInteractor))]
    public class GripRayInteractorToggle : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Which hand controls this Ray Interactor?")]
        private Hand controllerHand = Hand.Right;
        
        [SerializeField]
        [Tooltip("Enable the ray interactor on start?")]
        private bool enableOnStart = false;
        
        [SerializeField]
        [Tooltip("Require trigger press to detect stars?")]
        private bool requireTriggerForStars = true;
        
        [SerializeField]
        [Tooltip("Enable debug logs?")]
        private bool debugMode = false;
        
        private XRRayInteractor rayInteractor;
        
        public enum Hand
        {
            Left,
            Right
        }
        
        private void Awake()
        {
            // Get the XR Ray Interactor component
            rayInteractor = GetComponent<XRRayInteractor>();
            
            if (rayInteractor == null)
            {
                Debug.LogError($"[GripRayInteractorToggle] No XRRayInteractor found on {gameObject.name}!");
                enabled = false;
                return;
            }
        }
        
        private void OnEnable()
        {
            if (rayInteractor != null && requireTriggerForStars)
            {
                rayInteractor.hoverEntered.AddListener(OnHoverEntered);
            }
        }
        
        private void OnDisable()
        {
            if (rayInteractor != null && requireTriggerForStars)
            {
                rayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            }
        }
        
        private void Start()
        {
            // Check that the VRControllerInputs singleton exists
            if (VRControllerInputs.Instance == null)
            {
                Debug.LogError("[GripRayInteractorToggle] VRControllerInputs.Instance is not initialized!");
                enabled = false;
                return;
            }
            
            // Set initial state
            rayInteractor.enabled = enableOnStart;
        }
        
        private void Update()
        {
            // Verify that the singleton still exists
            if (VRControllerInputs.Instance == null)
            {
                return;
            }
            
            // Get grip state according to configured hand
            bool isGripPressed = controllerHand == Hand.Left 
                ? VRControllerInputs.Instance.IsLeftGripPressed 
                : VRControllerInputs.Instance.IsRightGripPressed;
            
            // Enable/disable the ray interactor
            if (rayInteractor.enabled != isGripPressed)
            {
                rayInteractor.enabled = isGripPressed;
                
                if (debugMode)
                {
                    Debug.Log($"[GripRayInteractorToggle] Ray Interactor {(isGripPressed ? "enabled" : "disabled")} ({controllerHand} hand)");
                }
            }
            
            // If trigger filtering is enabled, check for stars to remove from hover
            if (requireTriggerForStars && isGripPressed && rayInteractor.interactablesHovered.Count > 0)
            {
                bool triggerPressed = IsTriggerPressed();
                
                if (!triggerPressed)
                {
                    // Create a list to avoid modifying collection while iterating
                    var toRemove = new System.Collections.Generic.List<IXRHoverInteractable>();
                    
                    foreach (var interactable in rayInteractor.interactablesHovered)
                    {
                        if (interactable is MonoBehaviour mb)
                        {
                            var starInteractable = mb.GetComponent<StarInteractableSimple>();
                            if (starInteractable != null)
                            {
                                toRemove.Add(interactable);
                                
                                if (debugMode)
                                {
                                    Debug.Log($"[GripRayInteractorToggle] Removing star hover: {starInteractable.starName}");
                                }
                            }
                        }
                    }
                    
                    // Force cancel hover for all stars
                    if (toRemove.Count > 0)
                    {
                        rayInteractor.interactionManager.CancelInteractorHover(rayInteractor as IXRHoverInteractor);
                    }
                }
            }
        }
        
        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (!requireTriggerForStars)
                return;
            
            // Check if the hovered object is a star
            if (args.interactableObject is IXRHoverInteractable interactable)
            {
                if (interactable is MonoBehaviour mb)
                {
                    var starInteractable = mb.GetComponent<StarInteractableSimple>();
                    if (starInteractable != null)
                    {
                        // If trigger is not pressed, cancel the hover immediately
                        if (!IsTriggerPressed())
                        {
                            if (debugMode)
                            {
                                Debug.Log($"[GripRayInteractorToggle] Blocking star hover: {starInteractable.starName}");
                            }
                            
                            // Cancel hover by disabling interactable temporarily
                            var xrInteractable = interactable as XRBaseInteractable;
                            if (xrInteractable != null)
                            {
                                StartCoroutine(BlockStarInteraction(xrInteractable));
                            }
                        }
                        else if (debugMode)
                        {
                            Debug.Log($"[GripRayInteractorToggle] Allowing star hover: {starInteractable.starName}");
                        }
                    }
                }
            }
        }
        
        private System.Collections.IEnumerator BlockStarInteraction(XRBaseInteractable interactable)
        {
            // Temporarily disable the interactable
            interactable.enabled = false;
            yield return null; // Wait one frame
            interactable.enabled = true;
        }
        
        /// <summary>
        /// Change which hand controls this ray interactor
        /// </summary>
        public void SetControllerHand(Hand hand)
        {
            controllerHand = hand;
        }
        
        /// <summary>
        /// Force enable/disable the ray interactor regardless of grip state
        /// </summary>
        public void ForceEnable(bool enable)
        {
            if (rayInteractor != null)
            {
                rayInteractor.enabled = enable;
            }
        }
        
        /// <summary>
        /// Check if trigger is currently pressed on the configured hand
        /// </summary>
        public bool IsTriggerPressed()
        {
            if (VRControllerInputs.Instance == null)
                return false;
                
            return controllerHand == Hand.Left 
                ? VRControllerInputs.Instance.IsLeftTriggerPressed 
                : VRControllerInputs.Instance.IsRightTriggerPressed;
        }
    }
}
