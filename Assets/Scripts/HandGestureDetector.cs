using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>
/// Détecte les gestes des mains Meta Quest (pinch, grab, etc.)
/// Exemple d'utilisation pour déclencher des actions basées sur les gestes
/// </summary>
public class HandGestureDetector : MonoBehaviour
{
    [Header("Seuils de détection")]
    [Range(0f, 1f)]
    [Tooltip("Distance minimale pour détecter un pinch (pouce-index rapprochés)")]
    public float pinchThreshold = 0.03f;
    
    [Range(0f, 1f)]
    [Tooltip("Distance minimale pour détecter un grab (tous les doigts fermés)")]
    public float grabThreshold = 0.1f;
    
    [Header("Événements de debug")]
    [Tooltip("Afficher les logs de détection de gestes")]
    public bool debugLogs = true;
    
    // État des gestes
    private bool m_LeftPinching = false;
    private bool m_RightPinching = false;
    private bool m_LeftGrabbing = false;
    private bool m_RightGrabbing = false;
    
    private XRHandSubsystem m_HandSubsystem;
    
    void Update()
    {
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
            // Détecter les gestes pour chaque main
            DetectHandGestures(m_HandSubsystem.leftHand, ref m_LeftPinching, ref m_LeftGrabbing, "Left");
            DetectHandGestures(m_HandSubsystem.rightHand, ref m_RightPinching, ref m_RightGrabbing, "Right");
        }
    }
    
    void DetectHandGestures(XRHand hand, ref bool isPinching, ref bool isGrabbing, string handName)
    {
        if (!hand.isTracked)
            return;
        
        // Détecter le pinch (pouce et index rapprochés)
        bool pinchDetected = DetectPinch(hand);
        if (pinchDetected && !isPinching)
        {
            OnPinchStarted(handName);
            isPinching = true;
        }
        else if (!pinchDetected && isPinching)
        {
            OnPinchEnded(handName);
            isPinching = false;
        }
        
        // Détecter le grab (main fermée)
        bool grabDetected = DetectGrab(hand);
        if (grabDetected && !isGrabbing)
        {
            OnGrabStarted(handName);
            isGrabbing = true;
        }
        else if (!grabDetected && isGrabbing)
        {
            OnGrabEnded(handName);
            isGrabbing = false;
        }
    }
    
    bool DetectPinch(XRHand hand)
    {
        // Récupérer les positions du pouce et de l'index
        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose))
        {
            float distance = Vector3.Distance(thumbPose.position, indexPose.position);
            return distance < pinchThreshold;
        }
        return false;
    }
    
    bool DetectGrab(XRHand hand)
    {
        // Vérifier si tous les doigts sont fermés (proche de la paume)
        if (!hand.GetJoint(XRHandJointID.Palm).TryGetPose(out var palmPose))
            return false;
        
        int closedFingers = 0;
        XRHandJointID[] fingerTips = new XRHandJointID[]
        {
            XRHandJointID.IndexTip,
            XRHandJointID.MiddleTip,
            XRHandJointID.RingTip,
            XRHandJointID.LittleTip
        };
        
        foreach (var fingerTip in fingerTips)
        {
            if (hand.GetJoint(fingerTip).TryGetPose(out var tipPose))
            {
                float distance = Vector3.Distance(palmPose.position, tipPose.position);
                if (distance < grabThreshold)
                    closedFingers++;
            }
        }
        
        // Si au moins 3 doigts sur 4 sont fermés, c'est un grab
        return closedFingers >= 3;
    }
    
    // Événements appelés lors de la détection de gestes
    // Vous pouvez les remplacer par des UnityEvents pour les connecter dans l'Inspector
    
    void OnPinchStarted(string handName)
    {
        if (debugLogs)
            Debug.Log($"[HandGesture] {handName} hand PINCH started");
        
        // Ajoutez votre logique ici
        // Exemple : déclencher une action, sélectionner un objet, etc.
    }
    
    void OnPinchEnded(string handName)
    {
        if (debugLogs)
            Debug.Log($"[HandGesture] {handName} hand PINCH ended");
        
        // Ajoutez votre logique ici
    }
    
    void OnGrabStarted(string handName)
    {
        if (debugLogs)
            Debug.Log($"[HandGesture] {handName} hand GRAB started");
        
        // Ajoutez votre logique ici
        // Exemple : saisir un objet
    }
    
    void OnGrabEnded(string handName)
    {
        if (debugLogs)
            Debug.Log($"[HandGesture] {handName} hand GRAB ended");
        
        // Ajoutez votre logique ici
        // Exemple : relâcher un objet
    }
    
    // Méthodes publiques pour vérifier l'état des gestes depuis d'autres scripts
    
    public bool IsLeftHandPinching() => m_LeftPinching;
    public bool IsRightHandPinching() => m_RightPinching;
    public bool IsLeftHandGrabbing() => m_LeftGrabbing;
    public bool IsRightHandGrabbing() => m_RightGrabbing;
    
    /// <summary>
    /// Obtient la position d'un joint spécifique d'une main
    /// </summary>
    public bool TryGetJointPosition(bool leftHand, XRHandJointID jointId, out Vector3 position)
    {
        position = Vector3.zero;
        
        if (m_HandSubsystem == null || !m_HandSubsystem.running)
            return false;
        
        XRHand hand = leftHand ? m_HandSubsystem.leftHand : m_HandSubsystem.rightHand;
        
        if (!hand.isTracked)
            return false;
        
        if (hand.GetJoint(jointId).TryGetPose(out var pose))
        {
            position = pose.position;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Calcule la distance entre le pouce et l'index (utile pour les interactions de précision)
    /// </summary>
    public float GetPinchDistance(bool leftHand)
    {
        if (m_HandSubsystem == null || !m_HandSubsystem.running)
            return float.MaxValue;
        
        XRHand hand = leftHand ? m_HandSubsystem.leftHand : m_HandSubsystem.rightHand;
        
        if (!hand.isTracked)
            return float.MaxValue;
        
        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose))
        {
            return Vector3.Distance(thumbPose.position, indexPose.position);
        }
        
        return float.MaxValue;
    }
}

