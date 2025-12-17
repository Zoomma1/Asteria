using UnityEngine;
using LaserRayCasting;
using Feature1;

namespace LaserRayCasting
{
    /// <summary>
    /// Script qui affiche les informations d'une étoile lorsque le laser la touche.
    /// Affiche les informations via une UI 3D à côté de l'étoile.
    /// </summary>
    public class LaserStarInfo : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private LaserRayController laserController;
        [SerializeField] private LayerMask starLayer = -1;
        [SerializeField] private StarInfoUI starInfoUI;
        
        private StarData currentStar;
        
        void Start()
        {
            if (laserController == null)
                laserController = GetComponent<LaserRayController>();
            
            // Si l'UI n'est pas assignée, la chercher dans la scène
            if (starInfoUI == null)
                starInfoUI = FindObjectOfType<StarInfoUI>();
        }
        
        void Update()
        {
            if (laserController == null) return;
            
            // Vérifier si le laser est actif en vérifiant si le LineRenderer est activé
            LineRenderer lineRenderer = laserController.GetComponent<LineRenderer>();
            bool laserActive = lineRenderer != null && lineRenderer.enabled;
            
            if (laserActive)
            {
                // Effectuer un raycast dans la direction du laser
                Vector3 startPosition = transform.position;
                Vector3 direction = transform.forward;
                
                RaycastHit hit;
                if (Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity, starLayer))
                {
                    StarData starData = hit.collider.GetComponent<StarData>();
                    
                    if (starData != null && starData.star != null && starData != currentStar)
                    {
                        currentStar = starData;
                        ShowStarInfo(starData.star, starData.transform);
                    }
                    else if ((starData == null || starData.star == null) && currentStar != null)
                    {
                        // Le laser ne touche plus d'étoile ou l'étoile n'a pas de données
                        HideStarInfo();
                        currentStar = null;
                    }
                }
                else
                {
                    // Le laser ne touche rien
                    if (currentStar != null)
                    {
                        HideStarInfo();
                        currentStar = null;
                    }
                }
            }
            else
            {
                // Le laser est désactivé
                if (currentStar != null)
                {
                    HideStarInfo();
                    currentStar = null;
                }
            }
        }
        
        private void ShowStarInfo(Star star, Transform starTransform)
        {
            // Afficher dans la console (pour debug)
            string starNameStr = $"Étoile: {star.starName}";
            string constellationStr = $"Constellation: {star.constellationName} ({star.constellationID})";
            Debug.Log($"{starNameStr}\n{constellationStr}");
            
            // Afficher dans l'UI 3D
            if (starInfoUI != null)
            {
                starInfoUI.SetTargetStar(starTransform);
                starInfoUI.UpdateStarInfo(star.starName, star.constellationName);
            }
        }
        
        private void HideStarInfo()
        {
            if (starInfoUI != null)
            {
                starInfoUI.HideUI();
            }
        }
    }
}

