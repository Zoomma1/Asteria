using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace StarFieldInteraction
{
    /// <summary>
    /// Controls the visibility of the XR Ray Interactor line.
    /// Ray appears only when enabled and grip button is pressed.
    /// Can be controlled by MenuToggle to disable when menu is open.
    /// </summary>
    public class RayVisibilityController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private XRRayInteractor rayInteractor;
        
        [Header("Settings")]
        [SerializeField] 
        [Tooltip("Enable/Disable ray control from Inspector")]
        private bool enableRay = false;
        
        [SerializeField] 
        [Range(0f, 1f)]
        private float activationThreshold = 0.1f;
        
        [Header("Input")]
        [SerializeField] private bool useGripButton = true;
        [SerializeField] private bool useTriggerButton = false;
        
        private InputAction gripAction;
        
        private void Awake()
        {
            if (rayInteractor == null)
            {
                rayInteractor = GetComponent<XRRayInteractor>();
            }
            
            if (lineRenderer == null)
            {
                lineRenderer = GetComponentInChildren<LineRenderer>();
            }
            
            Debug.Log($"[RayVisibility] Awake - Enable Ray: {enableRay}, LineRenderer found: {lineRenderer != null}");
            
            // Auto-configure input button
            string binding = useTriggerButton ? 
                "<XRController>{RightHand}/trigger" : 
                "<XRController>{RightHand}/grip";
            
            gripAction = new InputAction(
                name: "Right Hand Input",
                type: InputActionType.Value,
                binding: binding
            );
            
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
                Debug.Log("[RayVisibility] LineRenderer disabled in Awake");
            }
            else
            {
                Debug.LogWarning("[RayVisibility] Line Renderer NOT FOUND! Check GameObject hierarchy.");
            }
        }
        
        private void OnEnable()
        {
            if (gripAction != null)
            {
                gripAction.Enable();
            }
        }
        
        private void OnDisable()
        {
            if (gripAction != null)
            {
                gripAction.Disable();
            }
        }
        
        private void Update()
        {
            if (lineRenderer == null) return;
            
            // If ray is disabled, keep LineRenderer off
            if (!enableRay)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }
                return;
            }
            
            if (gripAction == null) return;
            
            // Ray enabled: check input
            float gripValue = gripAction.ReadValue<float>();
            bool isGripPressed = gripValue > activationThreshold;
            
            lineRenderer.enabled = isGripPressed;
        }
        
        private void LateUpdate()
        {
            // Force disable if enableRay is false (override any other component)
            if (!enableRay && lineRenderer != null)
            {
                if (lineRenderer.enabled)
                {
                    Debug.LogWarning($"[RayVisibility] LineRenderer is ON! Force disabling. GameObject: {lineRenderer.gameObject.name}");
                    lineRenderer.enabled = false;
                }
            }
        }
        
        /// <summary>
        /// Enable or disable ray control (called by MenuToggle or other scripts)
        /// </summary>
        public void SetRayEnabled(bool enabled)
        {
            enableRay = enabled;
            
            if (!enabled && lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
        
        /// <summary>
        /// Get current ray enabled state
        /// </summary>
        public bool IsRayEnabled()
        {
            return enableRay;
        }
        
        private void OnValidate()
        {
            if (rayInteractor == null)
            {
                rayInteractor = GetComponent<XRRayInteractor>();
            }
            
            if (lineRenderer == null)
            {
                lineRenderer = GetComponentInChildren<LineRenderer>();
            }
        }
        
        private void OnDestroy()
        {
            if (gripAction != null)
            {
                gripAction.Disable();
                gripAction.Dispose();
            }
        }
    }
}

