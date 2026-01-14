using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using DTO;

namespace StarField
{
    public class StarFieldFromApi : MonoBehaviour
    {
        [SerializeField] private string apiUrl = "https://asteria-api.duckdns.org/AsteriaAPI/stars";
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private float radius = 50f;
        [SerializeField] private float minMag = -1.5f;
        [SerializeField] private int maxStars = 2000;
        [SerializeField] private float maxMag = 6.0f;
        [SerializeField] private float minSize = 0.02f;
        [SerializeField] private float maxSize = 0.08f;

        private void Start()
        {
            StartCoroutine(LoadStarsFromApi());
        }

        private IEnumerator LoadStarsFromApi()
        {
            if (starPrefab == null)
            {
                Debug.LogError("StarFieldFromApi : prefab d'étoile non assigné.");
                yield break;
            }

            var url = apiUrl + $"?&minMag={minMag}&maxMag={maxMag}&limit={maxStars}";
            url = url.Replace(",", "."); // s'assure que les paramètres décimaux sont au format point
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

    #if UNITY_2020_2_OR_NEWER
                if (req.result != UnityWebRequest.Result.Success)
    #else
                if (req.isNetworkError || req.isHttpError)
    #endif
                {
                    Debug.LogError("Erreur API étoiles : " + req.error);
                    Debug.LogError("URL : " + url);
                    yield break;
                }

                string json = req.downloadHandler.text;
                Debug.Log(json);
                StarApiResponseDto response = JsonUtility.FromJson<StarApiResponseDto>(json);
                if (response == null || response.stars == null)
                {
                    Debug.LogError("Réponse API invalide ou vide.");
                    yield break;
                }
                
                Debug.Log($"Chargé {response.stars.Length} étoiles depuis l'API.");

                foreach (StarDto s in response.stars)
                {
                    Vector3 dir = EquatorialToDirection((float)s.ra, (float)s.dec);
                    Vector3 pos = dir * radius;

                    GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, transform);
                    star.transform.rotation = Quaternion.LookRotation(-pos);

                    // taille selon magnitude
                    float t = Mathf.InverseLerp(maxMag, minMag, (float)s.mag);
                    float scale = Mathf.Lerp(minSize, maxSize, t);
                    star.transform.localScale = Vector3.one * scale;

                    star.name = s.proper;
                    
                    // Enlarge collider for easier XR interaction
                    Collider collider = star.GetComponent<Collider>();
                    if (collider != null && collider is BoxCollider boxCol)
                    {
                        boxCol.size = Vector3.one * 1.5f;
                    }
                    
                    // Register star in central registry for lookup by other scripts
                    string displayName = !string.IsNullOrEmpty(s.proper) ? s.proper : $"HIP {s.hip}";
                    if (StarFieldInteraction.StarDataRegistry.Instance != null)
                    {
                        StarFieldInteraction.StarDataRegistry.Instance.RegisterStar(
                            s.hip, displayName, s.ra, s.dec, s.mag, s.constellationName
                        );
                    }
                    
                    // Add XR interaction component
                    StarFieldInteraction.StarInteractableSimple interactable = star.AddComponent<StarFieldInteraction.StarInteractableSimple>();
                    
                    interactable.starName = displayName;
                    interactable.hipId = s.hip;
                    interactable.rightAscension = s.ra;
                    interactable.declination = s.dec;
                    interactable.magnitude = s.mag;
                    interactable.constellationName = s.constellationName;
                }
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
}
