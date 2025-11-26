using UnityEngine;
using LaserRayCasting;
using Feature1;

namespace LaserRayCasting
{
    /// <summary>
    /// Script qui affiche les informations d'une étoile lorsque le laser la touche.
    /// Affiche les informations via Debug.Log dans la console.
    /// </summary>
    public class LaserStarInfo : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private LaserRayController laserController;
        [SerializeField] private LayerMask starLayer = -1;
        
        private StarData currentStar;
        
        void Start()
        {
            if (laserController == null)
                laserController = GetComponent<LaserRayController>();
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
                Vector3 direction = -transform.forward;
                
                RaycastHit hit;
                if (Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity, starLayer))
                {
                    StarData starData = hit.collider.GetComponent<StarData>();
                    
                    if (starData != null && starData.star != null && starData != currentStar)
                    {
                        currentStar = starData;
                        ShowStarInfo(starData.star);
                    }
                    else if ((starData == null || starData.star == null) && currentStar != null)
                    {
                        // Le laser ne touche plus d'étoile ou l'étoile n'a pas de données
                        currentStar = null;
                    }
                }
                else
                {
                    // Le laser ne touche rien
                    if (currentStar != null)
                    {
                        currentStar = null;
                    }
                }
            }
            else
            {
                // Le laser est désactivé
                if (currentStar != null)
                {
                    currentStar = null;
                }
            }
        }
        
        private void ShowStarInfo(Star star)
        {
            string starNameStr = $"Étoile: {star.starName}";
            string constellationStr = $"Constellation: {star.constellationName} ({star.constellationID})";
            string historyStr = $"Histoire: {star.history}";
            
            Debug.Log($"{starNameStr}\n{constellationStr}\n{historyStr}");
        }
    }
}

