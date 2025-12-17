using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace LaserRayCasting
{
    /// <summary>
    /// Script qui affiche un rayon laser depuis la manette droite lorsque la gâchette est pressée.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LaserRayController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        [Tooltip("Action d'input pour la gâchette de la manette droite")]
        InputActionProperty m_TriggerAction;

        [Header("Rayon Laser")]
        [SerializeField]
        [Tooltip("Couleur du rayon laser")]
        Color m_LaserColor = Color.blue;

        [SerializeField]
        [Tooltip("Épaisseur du rayon laser")]
        float m_LaserWidth = 0.01f;

        [SerializeField]
        [Tooltip("Longueur maximale du rayon")]
        float m_MaxRayLength = 10f;

        [SerializeField]
        [Tooltip("Layer mask pour la détection de collisions")]
        LayerMask m_LayerMask = ~0;

        [SerializeField]
        [Tooltip("Offset de position pour centrer le laser sur la main (en coordonnées locales)")]
        Vector3 m_PositionOffset = Vector3.zero;

        private LineRenderer m_LineRenderer;
        private bool m_IsTriggerPressed = false;

        void Awake()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            SetupLineRenderer();
        }

        void OnEnable()
        {
            if (m_TriggerAction.action != null)
            {
                m_TriggerAction.action.Enable();
            }
        }

        void OnDisable()
        {
            if (m_TriggerAction.action != null)
            {
                m_TriggerAction.action.Disable();
            }
        }

        void Update()
        {
            // Vérifier si la gâchette est pressée
            bool triggerPressed = m_TriggerAction.action != null && m_TriggerAction.action.IsPressed();

            if (triggerPressed)
            {
                if (!m_IsTriggerPressed)
                {
                    // Début du pressage
                    m_IsTriggerPressed = true;
                    m_LineRenderer.enabled = true;
                }

                // Mettre à jour le rayon
                UpdateLaserRay();
            }
            else
            {
                if (m_IsTriggerPressed)
                {
                    // Fin du pressage
                    m_IsTriggerPressed = false;
                    m_LineRenderer.enabled = false;
                }
            }
        }

        void SetupLineRenderer()
        {
            m_LineRenderer.enabled = false;
            m_LineRenderer.startWidth = m_LaserWidth;
            m_LineRenderer.endWidth = m_LaserWidth;
            m_LineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_LineRenderer.startColor = m_LaserColor;
            m_LineRenderer.endColor = m_LaserColor;
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.useWorldSpace = true;
        }

        void UpdateLaserRay()
        {
            // Position de départ : centre de la main avec offset configurable
            Vector3 startPosition = transform.position + transform.TransformDirection(m_PositionOffset);
            
            // Direction : vers l'avant de la main (forward du transform)
            Vector3 direction = transform.forward;

            // Effectuer un raycast
            RaycastHit hit;
            Vector3 endPosition;

            if (Physics.Raycast(startPosition, direction, out hit, m_MaxRayLength, m_LayerMask))
            {
                // Si on touche quelque chose, le rayon s'arrête à l'impact
                endPosition = hit.point;
            }
            else
            {
                // Sinon, le rayon va jusqu'à la longueur maximale
                endPosition = startPosition + direction * m_MaxRayLength;
            }

            // Mettre à jour les positions du LineRenderer
            m_LineRenderer.SetPosition(0, startPosition);
            m_LineRenderer.SetPosition(1, endPosition);
        }

        void OnValidate()
        {
            // Mettre à jour les propriétés du LineRenderer dans l'éditeur
            if (m_LineRenderer == null)
                m_LineRenderer = GetComponent<LineRenderer>();

            if (m_LineRenderer != null)
            {
                m_LineRenderer.startWidth = m_LaserWidth;
                m_LineRenderer.endWidth = m_LaserWidth;
                m_LineRenderer.startColor = m_LaserColor;
                m_LineRenderer.endColor = m_LaserColor;
            }
        }
    }
}
