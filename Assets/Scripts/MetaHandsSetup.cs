using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>
/// Script pour faciliter l'activation/désactivation des mains Meta Quest et des contrôleurs
/// Attachez ce script à votre XR Origin pour gérer automatiquement le basculement
/// entre les contrôleurs et les mains en fonction du hand tracking
/// </summary>
public class MetaHandsSetup : MonoBehaviour
{
    [Header("Références des mains")]
    [Tooltip("Le GameObject qui contient les modèles de mains (Left Hand Model et Right Hand Model)")]
    public GameObject handsRoot;
    
    [Header("Références des contrôleurs")]
    [Tooltip("Le GameObject du contrôleur gauche (XR Controller Left)")]
    public GameObject leftController;
    
    [Tooltip("Le GameObject du contrôleur droit (XR Controller Right)")]
    public GameObject rightController;
    
    [Header("Options")]
    [Tooltip("Si activé, les contrôleurs seront désactivés automatiquement quand les mains sont trackées")]
    public bool autoSwitchToHands = true;
    
    [Tooltip("Si activé, force l'utilisation des mains dès le démarrage (désactive les contrôleurs)")]
    public bool forceHandsOnly = false;
    
    private XRHandSubsystem m_HandSubsystem;
    private bool m_LeftHandWasTracked = false;
    private bool m_RightHandWasTracked = false;
    
    void Start()
    {
        if (forceHandsOnly)
        {
            // Force l'utilisation des mains uniquement
            SetHandsActive(true);
            SetControllersActive(false);
        }
    }
    
    void Update()
    {
        if (!autoSwitchToHands || forceHandsOnly)
            return;
            
        // Récupérer le subsystem de hand tracking
        if (m_HandSubsystem == null || !m_HandSubsystem.running)
        {
            var subsystems = new System.Collections.Generic.List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            foreach (var subsystem in subsystems)
            {
                if (subsystem.running)
                {
                    m_HandSubsystem = subsystem;
                    break;
                }
            }
        }
        
        if (m_HandSubsystem != null && m_HandSubsystem.running)
        {
            bool leftHandTracked = m_HandSubsystem.leftHand.isTracked;
            bool rightHandTracked = m_HandSubsystem.rightHand.isTracked;
            bool anyHandTracked = leftHandTracked || rightHandTracked;
            
            // Si les mains sont trackées, activer les mains et désactiver les contrôleurs
            if (anyHandTracked && (!m_LeftHandWasTracked || !m_RightHandWasTracked))
            {
                SetHandsActive(true);
                SetControllersActive(false);
                Debug.Log($"[MetaHandsSetup] Mains détectées - Left: {leftHandTracked}, Right: {rightHandTracked}");
            }
            // Si les mains ne sont plus trackées, réactiver les contrôleurs
            else if (!anyHandTracked && (m_LeftHandWasTracked || m_RightHandWasTracked))
            {
                SetHandsActive(false);
                SetControllersActive(true);
                Debug.Log("[MetaHandsSetup] Mains perdues - Activation des contrôleurs");
            }
            
            m_LeftHandWasTracked = leftHandTracked;
            m_RightHandWasTracked = rightHandTracked;
        }
    }
    
    /// <summary>
    /// Active ou désactive les modèles de mains
    /// </summary>
    public void SetHandsActive(bool active)
    {
        if (handsRoot != null)
        {
            handsRoot.SetActive(active);
        }
    }
    
    /// <summary>
    /// Active ou désactive les contrôleurs
    /// </summary>
    public void SetControllersActive(bool active)
    {
        if (leftController != null)
        {
            leftController.SetActive(active);
        }
        if (rightController != null)
        {
            rightController.SetActive(active);
        }
    }
    
    /// <summary>
    /// Fonction pour basculer manuellement entre mains et contrôleurs
    /// </summary>
    public void ToggleHandsControllers()
    {
        bool handsAreActive = handsRoot != null && handsRoot.activeSelf;
        SetHandsActive(!handsAreActive);
        SetControllersActive(handsAreActive);
    }
}

