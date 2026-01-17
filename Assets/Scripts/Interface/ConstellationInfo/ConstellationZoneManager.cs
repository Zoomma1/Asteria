using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DTO;

namespace Interface.ConstellationInfo
{
    public class ConstellationZoneManager : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject constellationNameTextObj;
        [SerializeField] private GameObject idTextObj;
        
        void Start()
        {
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
        
        public void ShowConstellationInfo(ConstellationDto constellation)
        {
            if (infoPanel == null || constellation == null) 
            {
                Debug.LogError("InfoPanel or constellation is null");
                return;
            }
            
            infoPanel.SetActive(true);
            
            string nameStr = $"Constellation: {constellation.name}";
            string idStr = $"ID: {constellation.id}";
            
            Debug.Log($"Showing constellation info: {nameStr}, {idStr}");
            
            SetText(constellationNameTextObj, nameStr);
            SetText(idTextObj, idStr);
        }
        
        public void HideConstellationInfo()
        {
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
        
        private void SetText(GameObject textObj, string textValue)
        {
            if (textObj == null) return;
            
            Text text = textObj.GetComponent<Text>();
            if (text != null)
            {
                text.text = textValue;
                return;
            }
            
            TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = textValue;
            }
        }
    }
}
