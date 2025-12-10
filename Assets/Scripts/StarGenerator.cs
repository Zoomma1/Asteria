using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Feature1;

namespace Feature1
{
    public class StarGenerator : MonoBehaviour
{
    [Header("Prefab et données")]
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private TextAsset csvFile;
    
    [Header("Parent")]
    [SerializeField] private Transform starsParent; // Parent pour organiser les étoiles
    
    private List<Star> stars = new List<Star>();
    
    public void GenerateStars()
    {
        if (starPrefab == null)
        {
            Debug.LogError("Star Prefab n'est pas assigné !");
            return;
        }
        
        if (csvFile == null)
        {
            Debug.LogError("Fichier CSV n'est pas assigné !");
            return;
        }
        
        // Crée ou trouve le parent pour regrouper les étoiles
        if (starsParent == null)
        {
            GameObject parentObj = GameObject.Find("Stars");
            if (parentObj == null)
            {
                parentObj = new GameObject("Stars");
            }
            starsParent = parentObj.transform;
        }
        
        // Supprime les anciennes étoiles si elles existent
        ClearOldStars();
        
        // Parse le CSV
        ParseCSV(csvFile.text);
        
        // Génère les étoiles
        foreach (Star star in stars)
        {
            GameObject starInstance = Instantiate(starPrefab);
            
            // Assigne toujours le parent
            starInstance.transform.SetParent(starsParent);
            
            // Positionne l'étoile aux coordonnées du CSV
            starInstance.transform.position = star.position;
            
            // Ajoute les données de l'étoile au prefab
            StarData starData = starInstance.GetComponent<StarData>();
            if (starData == null)
            {
                starData = starInstance.AddComponent<StarData>();
            }
            
            starData.star = star;
            starInstance.name = star.starName;
        }
        
        Debug.Log($"Généré {stars.Count} étoiles");
    }
    
    private void ParseCSV(string csvText)
    {
        stars.Clear();
        
        string[] lines = csvText.Split('\n');
        
        // Ignore la première ligne (header)
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            // Parse en gérant les virgules dans History
            // Format: StarID,StarName,ConstellationID,ConstellationName,History,X,Y,Z
            string[] values = line.Split(',');
            
            if (values.Length >= 8)
            {
                try
                {
                    int starID = int.Parse(values[0].Trim());
                    string starName = values[1].Trim();
                    string constellationID = values[2].Trim();
                    string constellationName = values[3].Trim();
                    
                    // History peut contenir des virgules, donc on prend tout entre ConstellationName et les 3 derniers champs
                    // Les 3 derniers sont X, Y, Z
                    string history = "";
                    for (int j = 4; j < values.Length - 3; j++)
                    {
                        if (j > 4) history += ",";
                        history += values[j].Trim();
                    }
                    
                    string xStr = values[values.Length - 3].Trim();
                    string yStr = values[values.Length - 2].Trim();
                    string zStr = values[values.Length - 1].Trim();
                    
                    float x = float.Parse(xStr, CultureInfo.InvariantCulture);
                    float y = float.Parse(yStr, CultureInfo.InvariantCulture);
                    float z = float.Parse(zStr, CultureInfo.InvariantCulture);
                    
                    Vector3 position = new Vector3(x, y, z);
                    Star star = new Star(starID, starName, constellationID, constellationName, history, position);
                    stars.Add(star);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erreur lors du parsing de la ligne {i + 1}: {e.Message}\nLigne: {line}");
                }
            }
            else
            {
                Debug.LogWarning($"Ligne {i + 1} ignorée: pas assez de colonnes ({values.Length} au lieu de 8+)");
            }
        }
    }
    
    private void ClearOldStars()
    {
        if (starsParent != null)
        {
            // Supprime tous les enfants existants
            for (int i = starsParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(starsParent.GetChild(i).gameObject);
            }
        }
    }
}
}

