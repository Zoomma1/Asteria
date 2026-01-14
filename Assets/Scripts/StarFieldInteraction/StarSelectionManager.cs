using UnityEngine;
using TMPro;
using System.Collections;

namespace StarFieldInteraction
{
    /// <summary>
    /// Manages star selection UI display
    /// Singleton that shows star information in a canvas when a star is selected
    /// </summary>
    public class StarSelectionManager : MonoBehaviour
    {
        public static StarSelectionManager Instance { get; private set; }
        
        [Header("UI References")]
        [SerializeField]
        [Tooltip("The canvas to show when a star is selected")]
        private GameObject starInfoCanvas;
        
        [SerializeField]
        [Tooltip("VR Camera reference (Main Camera)")]
        private Transform vrCamera;
        
        [SerializeField]
        [Tooltip("Text component for star name")]
        private TextMeshProUGUI starNameText;
        
        [SerializeField]
        [Tooltip("Text component for star name")]
        private TextMeshProUGUI ConstellationNameText;
        
        [SerializeField]
        [Tooltip("Text component for HIP ID")]
        private TextMeshProUGUI hipIdText;
        
        [SerializeField]
        [Tooltip("Text component for Right Ascension")]
        private TextMeshProUGUI rightAscensionText;
        
        [SerializeField]
        [Tooltip("Text component for Declination")]
        private TextMeshProUGUI declinationText;
        
        [SerializeField]
        [Tooltip("Text component for Magnitude")]
        private TextMeshProUGUI magnitudeText;
        
        [Header("Settings")]
        [SerializeField]
        [Tooltip("Color to apply to selected star")]
        private Color selectedStarColor = Color.blue;
        
        [Header("Canvas Positioning")]
        [SerializeField]
        [Tooltip("Distance from camera")]
        private float distanceFromCamera = 1.5f;
        
        [SerializeField]
        [Tooltip("Vertical offset (negative = below center)")]
        private float verticalOffset = -0.4f;
        
        [SerializeField]
        [Tooltip("Smoothly move canvas to new position?")]
        private bool smoothTransition = true;
        
        [SerializeField]
        [Tooltip("Speed of smooth transition")]
        private float transitionSpeed = 5f;
        
        [SerializeField]
        [Tooltip("Enable fade in/out animation?")]
        private bool useFadeAnimation = true;
        
        [SerializeField]
        [Tooltip("Fade animation duration")]
        private float fadeDuration = 0.3f;
        
        [Header("UI Rotation")]
        [SerializeField]
        [Tooltip("Enable continuous rotation to face the camera (billboard effect)")]
        private bool enableBillboardRotation = true;
        
        [SerializeField]
        [Tooltip("Rotation speed for billboard effect")]
        private float billboardRotationSpeed = 10f;
        
        [SerializeField]
        [Tooltip("Enable continuous rotation around Y axis")]
        private bool enableContinuousRotation = false;
        
        [SerializeField]
        [Tooltip("Speed of continuous rotation (degrees per second)")]
        private float continuousRotationSpeed = 30f;
        
        [SerializeField]
        [Tooltip("Axis for continuous rotation")]
        private Vector3 rotationAxis = Vector3.up;
        
        private StarInteractableSimple currentlySelectedStar;
        private StarInteractableSimple currentlyHoveredStar;
        private Color originalStarColor;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private bool isPositioning = false;
        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning("[StarSelectionManager] Duplicate instance destroyed!");
                return;
            }
            Instance = this;
            
            // Try to find VR camera if not assigned
            if (vrCamera == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    vrCamera = mainCamera.transform;
                }
                else
                {
                    Debug.LogError("[StarSelectionManager] No VR Camera found! Please assign it manually.");
                }
            }
            
            // Hide canvas by default
            if (starInfoCanvas != null)
            {
                starInfoCanvas.SetActive(false);
                
                // Get or add CanvasGroup for fade animation
                if (useFadeAnimation)
                {
                    canvasGroup = starInfoCanvas.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = starInfoCanvas.AddComponent<CanvasGroup>();
                    }
                    canvasGroup.alpha = 0f;
                }
            }
        }
        
        private void Update()
        {
            // Check for B button press to select hovered star or close panel
            if (VRControllerInputs.Instance != null)
            {
                // Right secondary button (B) pressed this frame
                if (VRControllerInputs.Instance.RightSecondaryButtonDown)
                {
                    // If hovering a star, select it
                    if (currentlyHoveredStar != null)
                    {
                        OnStarSelected(currentlyHoveredStar);
                    }
                    // If no star hovered but panel is open, close it
                    else if (currentlySelectedStar != null && starInfoCanvas != null && starInfoCanvas.activeSelf)
                    {
                        Debug.Log("[StarSelectionManager] B button pressed - Closing info panel");
                        CloseInfoPanel();
                    }
                }
            }
            
            // Smoothly move canvas to target position
            if (isPositioning && starInfoCanvas != null && starInfoCanvas.activeSelf)
            {
                if (smoothTransition)
                {
                    starInfoCanvas.transform.position = Vector3.Lerp(
                        starInfoCanvas.transform.position,
                        targetPosition,
                        Time.deltaTime * transitionSpeed
                    );
                    
                    // Only apply target rotation if billboard is disabled
                    if (!enableBillboardRotation)
                    {
                        starInfoCanvas.transform.rotation = Quaternion.Slerp(
                            starInfoCanvas.transform.rotation,
                            targetRotation,
                            Time.deltaTime * transitionSpeed
                        );
                    }
                    
                    // Stop positioning when close enough
                    if (Vector3.Distance(starInfoCanvas.transform.position, targetPosition) < 0.01f)
                    {
                        starInfoCanvas.transform.position = targetPosition;
                        if (!enableBillboardRotation)
                        {
                            starInfoCanvas.transform.rotation = targetRotation;
                        }
                        isPositioning = false;
                    }
                }
                else
                {
                    starInfoCanvas.transform.position = targetPosition;
                    
                    // Only apply target rotation if billboard is disabled
                    if (!enableBillboardRotation)
                    {
                        starInfoCanvas.transform.rotation = targetRotation;
                    }
                    
                    isPositioning = false;
                }
            }
            
            // Apply UI rotation effects when canvas is active
            if (starInfoCanvas != null && starInfoCanvas.activeSelf)
            {
                ApplyUIRotation();
            }
        }
        
        /// <summary>
        /// Apply rotation effects to the UI canvas
        /// </summary>
        private void ApplyUIRotation()
        {
            if (starInfoCanvas == null || vrCamera == null)
            {
                return;
            }
            
            Quaternion finalRotation = starInfoCanvas.transform.rotation;
            
            // Billboard effect: always face the camera
            if (enableBillboardRotation)
            {
                Vector3 directionToCamera = vrCamera.position - starInfoCanvas.transform.position;
                if (directionToCamera != Vector3.zero)
                {
                    Quaternion billboardRotation = Quaternion.LookRotation(-directionToCamera);
                    finalRotation = Quaternion.Slerp(
                        finalRotation,
                        billboardRotation,
                        Time.deltaTime * billboardRotationSpeed
                    );
                }
            }
            
            // Continuous rotation around specified axis
            if (enableContinuousRotation)
            {
                finalRotation *= Quaternion.AngleAxis(
                    continuousRotationSpeed * Time.deltaTime,
                    rotationAxis
                );
            }
            
            starInfoCanvas.transform.rotation = finalRotation;
        }
        
        /// <summary>
        /// Called when a star is selected
        /// </summary>
        public void OnStarSelected(StarInteractableSimple star)
        {
            // Deselect previous star if any
            if (currentlySelectedStar != null)
            {
                DeselectCurrentStar();
            }
            
            currentlySelectedStar = star;
            
            // Change star color to blue
            Renderer renderer = star.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalStarColor = renderer.material.color;
                renderer.material.color = selectedStarColor;
            }
            
            // Update UI with star information
            UpdateStarInfoUI(star);
            
            // Position canvas in front of player
            PositionCanvasInFrontOfPlayer();
            
            // Show canvas with fade in
            if (starInfoCanvas != null)
            {
                starInfoCanvas.SetActive(true);
                
                if (useFadeAnimation && canvasGroup != null)
                {
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeCanvas(0f, 1f));
                }
            }
            
            Debug.Log($"[StarSelectionManager] Star selected: {star.starName}");
        }
        
        /// <summary>
        /// Position the canvas in front of the player, at the bottom of their view
        /// </summary>
        private void PositionCanvasInFrontOfPlayer()
        {
            if (vrCamera == null || starInfoCanvas == null)
            {
                Debug.LogWarning("[StarSelectionManager] VR Camera or Canvas not assigned!");
                return;
            }
            
            // Calculate position in front of camera, slightly below eye level
            Vector3 forward = vrCamera.forward;
            Vector3 right = vrCamera.right;
            Vector3 up = vrCamera.up;
            
            // Position: forward at distance, with vertical offset
            targetPosition = vrCamera.position + forward * distanceFromCamera + up * verticalOffset;
            
            // Rotation: face the player
            targetRotation = Quaternion.LookRotation(starInfoCanvas.transform.position - vrCamera.position);
            
            // Start positioning
            isPositioning = true;
        }
        
        /// <summary>
        /// Called when a star is hovered
        /// </summary>
        public void OnStarHovered(StarInteractableSimple star)
        {
            currentlyHoveredStar = star;
        }
        
        /// <summary>
        /// Called when a star hover exits
        /// </summary>
        public void OnStarUnhovered(StarInteractableSimple star)
        {
            if (currentlyHoveredStar == star)
            {
                currentlyHoveredStar = null;
            }
        }
        
        /// <summary>
        /// Called when a star is deselected (kept for compatibility)
        /// </summary>
        public void OnStarDeselected(StarInteractableSimple star)
        {
            if (currentlySelectedStar == star)
            {
                DeselectCurrentStar();
            }
        }
        
        /// <summary>
        /// Deselect the currently selected star
        /// </summary>
        private void DeselectCurrentStar()
        {
            if (currentlySelectedStar != null)
            {
                // Restore original color
                Renderer renderer = currentlySelectedStar.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white; // Reset to default
                }
                
                currentlySelectedStar = null;
            }
            
            // Hide canvas with fade out
            if (starInfoCanvas != null)
            {
                if (useFadeAnimation && canvasGroup != null)
                {
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeCanvasAndHide());
                }
                else
                {
                    starInfoCanvas.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Fade canvas in or out
        /// </summary>
        private IEnumerator FadeCanvas(float fromAlpha, float toAlpha)
        {
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                yield return null;
            }
            
            canvasGroup.alpha = toAlpha;
        }
        
        /// <summary>
        /// Fade out and then hide canvas
        /// </summary>
        private IEnumerator FadeCanvasAndHide()
        {
            yield return FadeCanvas(canvasGroup.alpha, 0f);
            starInfoCanvas.SetActive(false);
        }
        
        /// <summary>
        /// Update the UI with star information
        /// </summary>
        private void UpdateStarInfoUI(StarInteractableSimple star)
        {
            if (star == null)
            {
                Debug.LogError("[StarSelectionManager] Cannot update UI - star is null!");
                return;
            }
            
            Debug.Log($"[StarSelectionManager] Updating UI - Name: '{star.starName}', HIP: {star.hipId}, RA: {star.rightAscension:F2}, Dec: {star.declination:F2}, Mag: {star.magnitude:F2}");
            
            // Star Name
            if (starNameText != null)
            {
                string displayName = !string.IsNullOrEmpty(star.starName) || star.starName == "\"\""
                    ? star.starName 
                    : (star.hipId > 0 ? $"HIP {star.hipId}" : "Unknown Star");
                starNameText.text = displayName;
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] starNameText is NULL! Please assign in Inspector.");
            }
            
            // Constellation Name
            if (ConstellationNameText != null)
            {
                if (!string.IsNullOrEmpty(star.constellationName))
                {
                    ConstellationNameText.text = $"Constellation: {star.constellationName}";
                }
                else
                {
                    ConstellationNameText.text = "Constellation: Unknown";
                }
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] ConstellationNameText is NULL! Please assign in Inspector.");
            }
            
            // HIP ID
            if (hipIdText != null)
            {
                if (star.hipId > 0)
                {
                    hipIdText.text = $"HIP {star.hipId}";
                }
                else
                {
                    hipIdText.text = "HIP: Unknown";
                }
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] hipIdText is NULL! Please assign in Inspector.");
            }
            
            // Right Ascension (converted to hours:minutes:seconds format)
            if (rightAscensionText != null)
            {
                if (!double.IsNaN(star.rightAscension) && star.rightAscension >= 0)
                {
                    double ra = star.rightAscension;
                    int hours = (int)ra;
                    int minutes = (int)((ra - hours) * 60);
                    double seconds = ((ra - hours) * 60 - minutes) * 60;
                    rightAscensionText.text = $"RA: {hours:00}h {minutes:00}m {seconds:00.0}s";
                }
                else
                {
                    rightAscensionText.text = "RA: Unknown";
                }
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] rightAscensionText is NULL! Please assign in Inspector.");
            }
            
            // Declination (degrees:arcminutes:arcseconds format)
            if (declinationText != null)
            {
                if (!double.IsNaN(star.declination))
                {
                    double dec = star.declination;
                    int degrees = (int)dec;
                    double absRemainder = System.Math.Abs(dec - degrees);
                    int arcminutes = (int)(absRemainder * 60);
                    double arcseconds = (absRemainder * 60 - arcminutes) * 60;
                    string sign = dec >= 0 ? "+" : "";
                    declinationText.text = $"Dec: {sign}{degrees:00}° {arcminutes:00}' {arcseconds:00.0}\"";
                }
                else
                {
                    declinationText.text = "Dec: Unknown";
                }
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] declinationText is NULL! Please assign in Inspector.");
            }
            
            // Magnitude
            if (magnitudeText != null)
            {
                if (!double.IsNaN(star.magnitude))
                {
                    magnitudeText.text = $"Magnitude: {star.magnitude:F2}";
                }
                else
                {
                    magnitudeText.text = "Magnitude: Unknown";
                }
            }
            else
            {
                Debug.LogWarning("[StarSelectionManager] magnitudeText is NULL! Please assign in Inspector.");
            }
        }
        
        /// <summary>
        /// Get the currently selected star
        /// </summary>
        public StarInteractableSimple GetCurrentlySelectedStar()
        {
            return currentlySelectedStar;
        }
        
        /// <summary>
        /// Manually close the info panel
        /// </summary>
        public void CloseInfoPanel()
        {
            DeselectCurrentStar();
        }
    }
}
