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
            Debug.Log($"SELECT ENTERED → {displayName} - STAR SELECTED!");
            
            // Notify the StarSelectionManager
            if (StarSelectionManager.Instance != null)
            {
                StarSelectionManager.Instance.OnStarSelected(this);
            }
            else
            {
                // Fallback to yellow if no manager
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.yellow;
                }
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            
            string displayName = !string.IsNullOrEmpty(starName) ? starName : $"HIP {hipId}";
            Debug.Log($"SELECT EXITED → {displayName}");
            
            // Notify the StarSelectionManager
            if (StarSelectionManager.Instance != null)
            {
                StarSelectionManager.Instance.OnStarDeselected(this);
            }
            else
            {
                // Fallback to white if no manager
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }
}

