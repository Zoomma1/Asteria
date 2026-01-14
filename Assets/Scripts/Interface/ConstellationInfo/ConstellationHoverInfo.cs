using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DTO;

namespace Interface.ConstellationInfo
{
    public class ConstellationHoverInfo : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask constellationLayer = -1;
        
        [Header("UI - Text standard ou TextMeshPro")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject constellationNameTextObj;
        [SerializeField] private GameObject idTextObj;
        
        private ConstellationData currentConstellation;
        
        void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
        
        void Update()
        {
            // Vérifie que la souris est disponible
            if (Mouse.current == null) return;
            
            // Raycast depuis la caméra vers la souris (nouveau Input System)
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, constellationLayer))
            {
                ConstellationData constellationData = hit.collider.GetComponent<ConstellationData>();
                
                if (constellationData != null && constellationData != currentConstellation)
                {
                    currentConstellation = constellationData;
                    ShowConstellationInfo(constellationData.Constellation);
                }
            }
            else
            {
                if (currentConstellation != null)
                {
                    currentConstellation = null;
                    HideConstellationInfo();
                }
            }
        }
        
        private void ShowConstellationInfo(ConstellationDto constellation)
        {
            if (infoPanel == null) return;
            
            infoPanel.SetActive(true);
            
            string nameStr = $"Constellation: {constellation.name}";
            string idStr = $"ID: {constellation.id}";
            
            Debug.Log(nameStr);
            Debug.Log(idStr);
            
            // Support Text standard et TextMeshPro
            SetText(constellationNameTextObj, nameStr);
            SetText(idTextObj, idStr);
        }
        
        private void SetText(GameObject textObj, string textValue)
        {
            if (textObj == null) return;
            
            // Essaie Text standard d'abord
            Text text = textObj.GetComponent<Text>();
            if (text != null)
            {
                text.text = textValue;
                return;
            }
            
            // Sinon essaie TextMeshProUGUI
            TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = textValue;
            }
        }
        
        private void HideConstellationInfo()
        {
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
    }
}
