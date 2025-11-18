using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    [SerializeField] private int starCount = 500;
    [SerializeField] private float radius = 50f;
    [SerializeField] private GameObject starPrefab;

    private void Start()
    {
        if (starPrefab == null)
        {
            Debug.LogError("StarFieldGenerator : aucun prefab d'étoile assigné.");
            return;
        }

        for (int i = 0; i < starCount; i++)
        {
            // Direction aléatoire sur une sphère
            Vector3 dir = Random.onUnitSphere;
            Vector3 pos = dir * radius;

            // On instancie l'étoile
            GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, transform);

            // On oriente le quad vers le centre (0,0,0) pour toujours faire face à la caméra
            star.transform.rotation = Quaternion.LookRotation(-pos);
        }
    }
}
