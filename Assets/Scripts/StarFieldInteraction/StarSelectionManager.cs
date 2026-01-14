using UnityEngine;
using TMPro;

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
        [Tooltip("Text component for star name")]
        private TextMeshProUGUI starNameText;
        
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
        
        private StarInteractableSimple currentlySelectedStar;
        private Color originalStarColor;
        
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
            
            // Hide canvas by default
            if (starInfoCanvas != null)
            {
                starInfoCanvas.SetActive(false);
            }
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
            
            // Show canvas
            if (starInfoCanvas != null)
            {
                starInfoCanvas.SetActive(true);
            }
            
            Debug.Log($"[StarSelectionManager] Star selected: {star.starName}");
        }
        
        /// <summary>
        /// Called when a star is deselected
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
            
            // Hide canvas
            if (starInfoCanvas != null)
            {
                starInfoCanvas.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update the UI with star information
        /// </summary>
        private void UpdateStarInfoUI(StarInteractableSimple star)
        {
            // Star Name
            if (starNameText != null)
            {
                string displayName = !string.IsNullOrEmpty(star.starName) 
                    ? star.starName 
                    : $"Star HIP {star.hipId}";
                starNameText.text = displayName;
            }
            
            // HIP ID
            if (hipIdText != null)
            {
                hipIdText.text = $"HIP {star.hipId}";
            }
            
            // Right Ascension (converted to hours:minutes:seconds format)
            if (rightAscensionText != null)
            {
                double ra = star.rightAscension;
                int hours = (int)ra;
                int minutes = (int)((ra - hours) * 60);
                double seconds = ((ra - hours) * 60 - minutes) * 60;
                rightAscensionText.text = $"RA: {hours:00}h {minutes:00}m {seconds:00.0}s";
            }
            
            // Declination (degrees:arcminutes:arcseconds format)
            if (declinationText != null)
            {
                double dec = star.declination;
                int degrees = (int)dec;
                double absRemainder = System.Math.Abs(dec - degrees);
                int arcminutes = (int)(absRemainder * 60);
                double arcseconds = (absRemainder * 60 - arcminutes) * 60;
                string sign = dec >= 0 ? "+" : "";
                declinationText.text = $"Dec: {sign}{degrees:00}° {arcminutes:00}' {arcseconds:00.0}\"";
            }
            
            // Magnitude
            if (magnitudeText != null)
            {
                magnitudeText.text = $"Magnitude: {star.magnitude:F2}";
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
