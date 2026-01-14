using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace StarFieldInteraction
{
    /// <summary>
    /// Simple XR Toolkit interaction component for stars.
    /// Logs debug messages and changes color when hovered/selected.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class StarInteractableSimple : XRSimpleInteractable
    {
        [Header("Star Data")]
        public string starName;
        public int hipId;
        public double rightAscension;
        public double declination;
        public double magnitude;
        
        protected override void Awake()
        {
            base.Awake();
            
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogWarning($"[StarInteractable] {gameObject.name} has no Collider!");
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            
            string displayName = !string.IsNullOrEmpty(starName) ? starName : $"HIP {hipId}";
            Debug.Log($"HOVER ENTERED → {displayName} (RA: {rightAscension:F2}h, Dec: {declination:F2}°, Mag: {magnitude:F2})");
            
            // Notify the StarSelectionManager that this star is hovered
            if (StarSelectionManager.Instance != null)
            {
                StarSelectionManager.Instance.OnStarHovered(this);
            }
            
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.cyan;
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            
            string displayName = !string.IsNullOrEmpty(starName) ? starName : $"HIP {hipId}";
            Debug.Log($"HOVER EXITED → {displayName}");
            
            // Notify the StarSelectionManager that this star is no longer hovered
            if (StarSelectionManager.Instance != null)
            {
                StarSelectionManager.Instance.OnStarUnhovered(this);
            }
            
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            
            string displayName = !string.IsNullOrEmpty(starName) ? starName : $"HIP {hipId}";
            Debug.Log($"SELECT ENTERED → {displayName} (Note: Selection now done with B button)");
            
            // Selection is now handled by B button press, not by Select event
            // This is kept for debugging/logging purposes only
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            
            string displayName = !string.IsNullOrEmpty(starName) ? starName : $"HIP {hipId}";
            Debug.Log($"SELECT EXITED → {displayName}");
            
            // Selection is now handled by B button press, not by Select event
            // This is kept for debugging/logging purposes only
        }
    }
}

