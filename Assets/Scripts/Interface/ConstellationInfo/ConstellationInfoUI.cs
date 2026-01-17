using UnityEngine;
using TMPro;
using System.Collections;

namespace Interface.ConstellationInfo
{
    /// <summary>
    /// Script qui gère l'affichage d'une UI 3D à côté d'une constellation sélectionnée.
    /// L'UI affiche le nom de la constellation et son ID.
    /// </summary>
    public class ConstellationInfoUI : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private TextMeshProUGUI constellationNameText;
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private Canvas canvas;
        
        [Header("Paramètres")]
        [SerializeField] private Transform targetConstellation;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.3f, 0);
        [SerializeField] private bool lookAtCamera = true;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float hideDelay = 0.5f; // Délai en secondes avant de masquer l'UI
        [SerializeField] private float distanceFromConstellation = 0.5f; // Distance de l'UI par rapport à la constellation
        [SerializeField] private bool useCameraRelativePosition = true; // Positionner l'UI entre la caméra et la constellation
        
        // Nouveau : choisir le mode de rendu du Canvas depuis l'inspecteur
        [Header("Canvas Render Mode")]
        [SerializeField] private RenderMode desiredRenderMode = RenderMode.ScreenSpaceOverlay;
        
        private Coroutine _hideCoroutine;
        
        private void Awake()
        {
            // Si le canvas n'est pas assigné, le chercher sur le même GameObject
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            
            // Trouver la caméra principale si elle n'est pas assignée
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            // Si toujours null, chercher la caméra XR
            if (mainCamera == null)
            {
                var xrOrigin = UnityEngine.Object.FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
                if (xrOrigin != null && xrOrigin.Camera != null)
                    mainCamera = xrOrigin.Camera;
            }

            // Configurer le canvas selon le mode demandé (ne plus forcer WorldSpace systématiquement)
            if (canvas != null && canvas.renderMode != desiredRenderMode)
            {
                canvas.renderMode = desiredRenderMode;

                // Si on choisit ScreenSpace-Camera, assigner la caméra
                if (desiredRenderMode == RenderMode.ScreenSpaceCamera)
                {
                    if (mainCamera == null)
                        mainCamera = Camera.main; // tentative

                    canvas.worldCamera = mainCamera;
                }

                // Si on choisit WorldSpace, assurer une échelle raisonnable par défaut
                if (desiredRenderMode == RenderMode.WorldSpace)
                {
                    // éviter une échelle nulle ou extrême par accident
                    if (canvas.transform.localScale == Vector3.zero)
                        canvas.transform.localScale = Vector3.one * 0.01f;

                    // NB: pour WorldSpace, l'utilisateur doit positionner le RectTransform dans la scène
                }
            }
        }
        
        private void Update()
        {
            if (targetConstellation == null)
            {
                // Ne pas masquer immédiatement, laisser la coroutine gérer le délai
                return;
            }
            
            // Afficher l'UI
            if (canvas != null)
                canvas.gameObject.SetActive(true);
            
            // Calculer la position de l'UI (position en espace monde)
            Vector3 targetPosition = CalculateUIPosition();

            // Si le canvas est en WorldSpace, on place l'UI dans le monde et on la tourne vers la caméra
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                transform.position = targetPosition;

                if (lookAtCamera && mainCamera != null)
                {
                    Vector3 directionToCamera = mainCamera.transform.position - transform.position;
                    if (directionToCamera != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(-directionToCamera);
                }
            }
            else // Screen Space (Overlay ou Camera)
            {
                // Déterminer la caméra à utiliser pour la conversion écran et pour la fonction ScreenPointToLocalPointInRectangle
                // - Pour ScreenSpace-Overlay, ScreenPointToLocalPointInRectangle doit recevoir null
                // - Pour ScreenSpace-Camera, on passe canvas.worldCamera
                Camera camForRect;
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    camForRect = canvas.worldCamera;
                }
                else if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    camForRect = null; // overlay -> no camera for the rect conversion
                }
                else
                {
                    camForRect = mainCamera != null ? mainCamera : Camera.main;
                }

                // Caméra utilisée pour WorldToScreenPoint (doit être une vraie Camera)
                Camera camForScreenPoint = (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
                    ? canvas.worldCamera
                    : (mainCamera != null ? mainCamera : Camera.main);

                Vector3 screenPoint = camForScreenPoint != null ? camForScreenPoint.WorldToScreenPoint(targetPosition) : Vector3.zero;

                // Récupérer RectTransform du canvas et du panneau UI
                RectTransform canvasRect = canvas != null ? canvas.GetComponent<RectTransform>() : null;
                RectTransform myRect = GetComponent<RectTransform>();

                if (canvasRect != null && myRect != null)
                {
                    Vector2 localPoint;
                    // Pour ScreenSpace-Overlay, camForConversion peut être null — c'est acceptable
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, camForRect, out localPoint))
                    {
                        myRect.localPosition = localPoint;
                        // Optionnel : orienter vers la caméra (pour éléments 2D ce n'est généralement pas nécessaire)
                        if (lookAtCamera && mainCamera != null)
                        {
                            Vector3 dir = mainCamera.transform.position - myRect.position;
                            if (dir != Vector3.zero)
                                myRect.rotation = Quaternion.LookRotation(-dir);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Calcule la position optimale de l'UI par rapport à la constellation et à la caméra
        /// </summary>
        private Vector3 CalculateUIPosition()
        {
            Vector3 basePosition = targetConstellation.position + offset;
            
            if (useCameraRelativePosition && mainCamera != null)
            {
                // Calculer la direction de la caméra vers la constellation
                Vector3 directionFromCamera = (targetConstellation.position - mainCamera.transform.position).normalized;
                
                // Positionner l'UI entre la caméra et la constellation, légèrement décalée vers le haut
                Vector3 cameraRelativeOffset = directionFromCamera * distanceFromConstellation;
                cameraRelativeOffset.y += offset.y; // Ajouter l'offset vertical
                
                return targetConstellation.position + cameraRelativeOffset;
            }
            else
            {
                // Utiliser simplement l'offset configuré
                return basePosition;
            }
        }
        
        /// <summary>
        /// Met à jour l'UI avec les informations de la constellation
        /// </summary>
        public void UpdateConstellationInfo(string constellationName, string id)
        {
            if (constellationNameText != null)
            {
                Debug.Log(constellationNameText);
                constellationNameText.text = constellationName;
            }
            
            if (idText != null)
                idText.text = id;
        }
        
        /// <summary>
        /// Définit la constellation cible à suivre
        /// </summary>
        public void SetTargetConstellation(Transform constellationTransform)
        {
            // Annuler le masquage en cours si une nouvelle constellation est sélectionnée
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            
            targetConstellation = constellationTransform;
            
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
            if (_hideCoroutine != null)
                return;
            
            // Arrêter de suivre la constellation immédiatement, mais garder l'UI visible pendant le délai
            targetConstellation = null;
            
            // Démarrer la coroutine de masquage avec délai
            _hideCoroutine = StartCoroutine(HideUIAfterDelay());
        }
        
        private IEnumerator HideUIAfterDelay()
        {
            // Attendre le délai configuré
            yield return new WaitForSeconds(hideDelay);
            
            // Masquer l'UI seulement si aucune nouvelle constellation n'a été sélectionnée
            if (targetConstellation == null && canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }
            
            _hideCoroutine = null;
        }
        
        /// <summary>
        /// Masque l'UI immédiatement (sans délai)
        /// </summary>
        public void HideUIImmediate()
        {
            // Annuler la coroutine de masquage si elle est en cours
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            
            targetConstellation = null;
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }
    }
}
