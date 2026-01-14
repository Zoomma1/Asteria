using UnityEngine;
using LaserRayCasting;
using Interface.ConstellationInfo;
using DTO;

namespace Interface.ConstellationInfo
{
    /// <summary>
    /// Script qui affiche les informations d'une constellation lorsque le laser la touche.
    /// Affiche les informations via une UI 3D à côté de la constellation.
    /// </summary>
    public class LaserConstellationInfo : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private LaserRayController laserController;
        [SerializeField] private LayerMask constellationLayer = -1;
        [SerializeField] private ConstellationInfoUI constellationInfoUI;
        
        private ConstellationData currentConstellation;
        
        void Start()
        {
            if (laserController == null)
                laserController = GetComponent<LaserRayController>();
            
            // Si l'UI n'est pas assignée, la chercher dans la scène
            if (constellationInfoUI == null)
                constellationInfoUI = FindObjectOfType<ConstellationInfoUI>();
        }
        
                void Update()
        {
            if (laserController == null) return;
            
            // Vérifier si le laser est actif en vérifiant si le LineRenderer est activé
            LineRenderer lineRenderer = laserController.GetComponent<LineRenderer>();
            bool laserActive = lineRenderer != null && lineRenderer.enabled;
            
            if (laserActive)
            {
                Debug.Log("Laser is active");
                // Effectuer un raycast dans la direction du laser
                Vector3 startPosition = transform.position;
                Vector3 direction = transform.forward;
                
                RaycastHit hit;
                if (Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity, constellationLayer))
                {
                    Debug.Log("Laser hit: " + hit.collider.name);
                    ConstellationData constellationData = hit.collider.GetComponent<ConstellationData>();
                    
                    if (constellationData != null && constellationData.Constellation != null && constellationData != currentConstellation)
                    {
                        Debug.Log("Found new constellation: " + constellationData.Constellation.name);
                        currentConstellation = constellationData;
                        ShowConstellationInfo(constellationData.Constellation, constellationData.transform);
                    }
                    else if ((constellationData == null || constellationData.Constellation == null) && currentConstellation != null)
                    {
                        // Le laser ne touche plus de constellation ou la constellation n'a pas de données
                        Debug.Log("Laser no longer on constellation");
                        HideConstellationInfo();
                        currentConstellation = null;
                    }
                }
                else
                {
                    // Le laser ne touche rien
                    if (currentConstellation != null)
                    {
                        Debug.Log("Laser not hitting anything");
                        HideConstellationInfo();
                        currentConstellation = null;
                    }
                }
            }
            else
            {
                // Le laser est désactivé
                if (currentConstellation != null)
                {
                    Debug.Log("Laser deactivated");
                    HideConstellationInfo();
                    currentConstellation = null;
                    
                }
            }
        }
        
        private void ShowConstellationInfo(ConstellationDto constellation, Transform constellationTransform)
        {
            // Afficher dans la console (pour debug)
            string constellationNameStr = $"Constellation: {constellation.name}";
            string idStr = $"ID: {constellation.id}";
            Debug.Log($"{constellationNameStr}\n{idStr}");
            
            // Afficher dans l'UI 3D
            if (constellationInfoUI != null)
            {
                constellationInfoUI.SetTargetConstellation(constellationTransform);
                constellationInfoUI.UpdateConstellationInfo(constellation.name, constellation.id);
            }
        }
        
        private void HideConstellationInfo()
        {
            if (constellationInfoUI != null)
            {
                constellationInfoUI.HideUI();
            }
        }
    }
}
