using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Feature1;

namespace Feature1
{
    public class StarHoverInfo : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask starLayer = -1;
    
    [Header("UI - Text standard ou TextMeshPro")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject starNameTextObj;
    [SerializeField] private GameObject constellationTextObj;
    [SerializeField] private GameObject historyTextObj;
    
    private StarData currentStar;
    
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
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, starLayer))
        {
            StarData starData = hit.collider.GetComponent<StarData>();
            
            if (starData != null && starData != currentStar)
            {
                currentStar = starData;
                ShowStarInfo(starData.star);
            }
        }
        else
        {
            if (currentStar != null)
            {
                currentStar = null;
                HideStarInfo();
            }
        }
    }
    
    private void ShowStarInfo(Star star)
    {
        if (infoPanel == null) return;
        
        infoPanel.SetActive(true);
        
        string starNameStr = $"Étoile: {star.starName}";
        string constellationStr = $"Constellation: {star.constellationName} ({star.constellationID})";
        string historyStr = $"Histoire: {star.history}";
        
        // Support Text standard et TextMeshPro
        SetText(starNameTextObj, starNameStr);
        SetText(constellationTextObj, constellationStr);
        SetText(historyTextObj, historyStr);
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
    
    private void HideStarInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
}

