using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using DTO;

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

        // HIP -> position du point dans la sphère
        Dictionary<int, Vector3> hipToPos = new Dictionary<int, Vector3>();

        // ---- Instanciation des étoiles ----
        foreach (StarDto star in constellation.stars.stars.stars)
        {
            Vector3 dir = EquatorialToDirection((float)star.ra, (float)star.dec);
            Vector3 pos = dir * radius;

            GameObject starGO = Instantiate(starPrefab, pos, Quaternion.identity, constelRoot.transform);
            starGO.name = $"{constellation.name}_HIP{star.hip}";

            // billboard vers le centre
            starGO.transform.rotation = Quaternion.LookRotation(-pos);

            hipToPos[star.hip] = pos;
        }

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
                
                Debug.Log("Linking " + link.fromStarHip);
                
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
