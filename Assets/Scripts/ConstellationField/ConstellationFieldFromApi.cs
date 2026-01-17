using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using DTO;
using Interface.ConstellationInfo;

public class ConstellationFieldFromApi : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private string apiUrl = "http://localhost:8080/api/constellations";

    [Header("Sky settings")]
    [SerializeField] private float radius = 50f;
    [SerializeField] private GameObject starPrefab;

    [Header("Lines")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.02f;

    private void Start()
    {
        StartCoroutine(LoadConstellations());
    }

    private IEnumerator LoadConstellations()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(apiUrl))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching constellations: " + req.error);
                yield break;
            }

            string json = req.downloadHandler.text;
            Debug.Log("JSON: " + json.Substring(0, Mathf.Min(5000, json.Length)));

            ConstellationApiResponseDto response =
                JsonUtility.FromJson<ConstellationApiResponseDto>(json);

            if (response == null || response.constellations == null)
            {
                Debug.LogError("Failed to parse constellations JSON");
                yield break;
            }

            foreach (var constellation in response.constellations)
            {
                CreateConstellation(constellation);
            }
        }
    }

    private void CreateConstellation(ConstellationDto constellation)
    {
        if (constellation.stars == null ||
            constellation.stars.stars == null ||
            constellation.stars.stars.stars == null ||
            constellation.stars.stars.stars.Count == 0)
        {
            return;
        }

        // Parent pour ranger la constellation
        GameObject constelRoot = new GameObject($"Constellation_{constellation.name}");
        constelRoot.transform.SetParent(transform, false);

        // Ajouter le composant ConstellationData
        ConstellationData data = constelRoot.AddComponent<ConstellationData>();
        data.Init(constellation);

        // Ajouter un collider trigger pour la zone
        SphereCollider collider = constelRoot.AddComponent<SphereCollider>();
        collider.isTrigger = true;

        // HIP -> position du point dans la sphère
        Dictionary<int, Vector3> hipToPos = new Dictionary<int, Vector3>();

        // ---- Instanciation des étoiles ----
        foreach (StarDto star in constellation.stars.stars.stars)
        {
            Vector3 dir = EquatorialToDirection((float)star.ra, (float)star.dec);
            Vector3 pos = dir * radius;
            
            star.constellationName = constellation.name;

            GameObject starGO = Instantiate(starPrefab, pos, Quaternion.identity, constelRoot.transform);
            starGO.name = $"{constellation.name}_HIP{star.hip}";

            // billboard vers le centre
            starGO.transform.rotation = Quaternion.LookRotation(-pos);

            hipToPos[star.hip] = pos;
            
            // Register star in central registry for auto-population
            string displayName = !string.IsNullOrEmpty(star.proper) ? star.proper : $"HIP {star.hip}";
            if (StarFieldInteraction.StarDataRegistry.Instance != null)
            {
                StarFieldInteraction.StarDataRegistry.Instance.RegisterStar(
                    star.hip, displayName, star.ra, star.dec, star.mag, star.constellationName
                );
            }
        }

        // Calculer la position moyenne des étoiles pour centrer la constellation
        Vector3 averagePos = Vector3.zero;
        foreach (var pos in hipToPos.Values)
        {
            averagePos += pos;
        }
        averagePos /= hipToPos.Count;

        // Positionner le root au centre de la constellation
        constelRoot.transform.position = averagePos;

        // Repositionner les étoiles relativement au nouveau centre
        foreach (var kvp in hipToPos)
        {
            Vector3 newLocalPos = kvp.Value - averagePos;
            // Mettre à jour la position de l'étoile
            Transform starTransform = constelRoot.transform.Find($"{constellation.name}_HIP{kvp.Key}");
            if (starTransform != null)
            {
                starTransform.localPosition = newLocalPos;
                // Ajuster la rotation billboard vers le centre relatif
                starTransform.rotation = Quaternion.LookRotation(-newLocalPos);
            }
        }

        // Calculer le rayon du collider basé sur la distance maximale entre les étoiles
        List<Vector3> localPositions = new List<Vector3>();
        foreach (Transform child in constelRoot.transform)
        {
            if (child.name.Contains("_HIP"))
            {
                localPositions.Add(child.localPosition);
            }
        }
        
        float maxDistance = 0f;
        for (int i = 0; i < localPositions.Count; i++)
        {
            for (int j = i + 1; j < localPositions.Count; j++)
            {
                float dist = Vector3.Distance(localPositions[i], localPositions[j]);
                maxDistance = Mathf.Max(maxDistance, dist);
            }
        }
        collider.radius = maxDistance / 2f + 10f;

        // ---- Tracé des segments à partir de linkedStars ----
        if (constellation.stars.linkedStars != null &&
            constellation.stars.linkedStars.stars != null)
        {
            int segmentIndex = 0;

            foreach (var link in constellation.stars.linkedStars.stars)
            {
                if (!hipToPos.TryGetValue(link.fromStarHip, out Vector3 p1) ||
                    !hipToPos.TryGetValue(link.toStarHip, out Vector3 p2))
                {
                    Debug.LogWarning($"Missing HIP in positions for constellation {constellation.name} : {link.fromStarHip} -> {link.toStarHip}");
                    continue;
                }
                
                GameObject lineObj = new GameObject($"{constellation.name}_Seg_{segmentIndex++}");
                lineObj.transform.SetParent(constelRoot.transform, false);

                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.useWorldSpace = true;
                lr.widthMultiplier = lineWidth;
                lr.material = lineMaterial;
                lr.loop = false;

                lr.SetPosition(0, p1);
                lr.SetPosition(1, p2);
            }
        } else {
            Debug.LogWarning($"No linked stars for constellation {constellation.name}");
        }
    }

    private Vector3 EquatorialToDirection(float raHours, float decDeg)
    {
        float raRad = raHours * 15f * Mathf.Deg2Rad;
        float decRad = decDeg * Mathf.Deg2Rad;

        float y = Mathf.Sin(decRad);
        float r = Mathf.Cos(decRad);

        float z = r * Mathf.Cos(raRad);
        float x = r * Mathf.Sin(raRad);

        return new Vector3(x, y, z).normalized;
    }
}
