using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace LaserRayCasting
{
    /// <summary>
    /// Script qui gère l'affichage d'une UI 3D à côté d'une étoile sélectionnée.
    /// L'UI affiche le nom de l'étoile et sa constellation.
    /// </summary>
    public class StarInfoUI : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private TextMeshProUGUI starNameText;
        [SerializeField] private TextMeshProUGUI constellationText;
        [SerializeField] private Canvas canvas;
        
        [Header("Paramètres")]
        [SerializeField] private Transform targetStar;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.3f, 0);
        [SerializeField] private bool lookAtCamera = true;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float hideDelay = 0.5f; // Délai en secondes avant de masquer l'UI
        [SerializeField] private float distanceFromStar = 0.5f; // Distance de l'UI par rapport à l'étoile
        [SerializeField] private bool useCameraRelativePosition = true; // Positionner l'UI entre la caméra et l'étoile
        
        private Coroutine hideCoroutine;
        
        private void Awake()
        {
            // Si le canvas n'est pas assigné, le chercher sur le même GameObject
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            
            // Configurer le canvas en World Space si ce n'est pas déjà fait
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                canvas.renderMode = RenderMode.WorldSpace;
            }
            
            // Trouver la caméra principale si elle n'est pas assignée
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            // Si toujours null, chercher la caméra XR
            if (mainCamera == null)
            {
                var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
                if (xrOrigin != null && xrOrigin.Camera != null)
                    mainCamera = xrOrigin.Camera;
            }
        }
        
        private void Update()
        {
            if (targetStar == null)
            {
                // Ne pas masquer immédiatement, laisser la coroutine gérer le délai
                return;
            }
            
            // Afficher l'UI
            if (canvas != null)
                canvas.gameObject.SetActive(true);
            
            // Calculer la position de l'UI
            Vector3 targetPosition = CalculateUIPosition();
            transform.position = targetPosition;
            
            // Orienter l'UI vers la caméra
            if (lookAtCamera && mainCamera != null)
            {
                Vector3 directionToCamera = mainCamera.transform.position - transform.position;
                if (directionToCamera != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(-directionToCamera);
                }
            }
        }
        
        /// <summary>
        /// Calcule la position optimale de l'UI par rapport à l'étoile et à la caméra
        /// </summary>
        private Vector3 CalculateUIPosition()
        {
            Vector3 basePosition = targetStar.position + offset;
            
            if (useCameraRelativePosition && mainCamera != null)
            {
                // Calculer la direction de la caméra vers l'étoile
                Vector3 directionFromCamera = (targetStar.position - mainCamera.transform.position).normalized;
                
                // Positionner l'UI entre la caméra et l'étoile, légèrement décalée vers le haut
                Vector3 cameraRelativeOffset = directionFromCamera * distanceFromStar;
                cameraRelativeOffset.y += offset.y; // Ajouter l'offset vertical
                
                return targetStar.position + cameraRelativeOffset;
            }
            else
            {
                // Utiliser simplement l'offset configuré
                return basePosition;
            }
        }
        
        /// <summary>
        /// Met à jour l'UI avec les informations de l'étoile
        /// </summary>
        public void UpdateStarInfo(string starName, string constellationName)
        {
            if (starNameText != null)
                starNameText.text = starName;
            
            if (constellationText != null)
                constellationText.text = constellationName;
        }
        
        /// <summary>
        /// Définit l'étoile cible à suivre
        /// </summary>
        public void SetTargetStar(Transform starTransform)
        {
            // Annuler le masquage en cours si une nouvelle étoile est sélectionnée
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
            
            targetStar = starTransform;
            
            // Afficher immédiatement l'UI
            if (canvas != null)
                canvas.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Masque l'UI après un délai
        /// </summary>
        public void HideUI()
        {
            // Si une coroutine de masquage est déjà en cours, ne pas en créer une nouvelle
            if (hideCoroutine != null)
                return;
            
            // Arrêter de suivre l'étoile immédiatement, mais garder l'UI visible pendant le délai
            targetStar = null;
            
            // Démarrer la coroutine de masquage avec délai
            hideCoroutine = StartCoroutine(HideUIAfterDelay());
        }
        
        private IEnumerator HideUIAfterDelay()
        {
            // Attendre le délai configuré
            yield return new WaitForSeconds(hideDelay);
            
            // Masquer l'UI seulement si aucune nouvelle étoile n'a été sélectionnée
            if (targetStar == null && canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }
            
            hideCoroutine = null;
        }
        
        /// <summary>
        /// Masque l'UI immédiatement (sans délai)
        /// </summary>
        public void HideUIImmediate()
        {
            // Annuler la coroutine de masquage si elle est en cours
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
            
            targetStar = null;
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }
    }
}

