using UnityEngine;

namespace Menu
{
    public class LazyFollow : MonoBehaviour
    {
        [Header("References")]
        public Transform cameraToFollow;
        [Tooltip("Caméra utilisée pour déterminer si le menu est dans le champ de vision. Si null, on tente de la déduire depuis cameraToFollow.")]
        public Camera viewCamera;

        [Header("Follow (target pose)")]
        public float distance = 3.0f;
        public float xOffset = 0.0f;
        public float yOffset = 4f;
        public Vector3 rotationOffset = Vector3.zero;
        public float smoothSpeed = 2.0f;

        [Header("Initialisation (pose fixe au lancement)")]
        public bool applyInitialTransformOnStart = true;
        public Vector3 initialPosition;
        public Vector3 initialEulerRotation;

        [Header("Visibilité")]
        [Tooltip("Marge pour la détection dans le viewport. 0 = strict (0..1). 0.05 = tolère un peu avant de considérer 'hors champ'.")]
        [Range(0f, 0.25f)]
        public float viewportMargin = 0.03f;

        void Start()
        {
            if (applyInitialTransformOnStart)
            {
                transform.position = initialPosition;
                transform.rotation = Quaternion.Euler(initialEulerRotation);
            }

            if (viewCamera == null && cameraToFollow != null)
            {
                // Souvent, la caméra VR est sur cameraToFollow ou dans ses enfants.
                viewCamera = cameraToFollow.GetComponentInChildren<Camera>();
            }
        }

        void Update()
        {
            if (cameraToFollow == null) return;

            // Ne rien faire si le menu est visible - il reste fixe
            if (IsMenuVisible())
            {
                return;
            }

            // Le menu n'est plus visible : on le déplace et on le fait tourner vers le joueur
            // Calculate target position
            Vector3 localOffset = new Vector3(xOffset, yOffset, distance);
            Vector3 targetPosition = cameraToFollow.position + cameraToFollow.TransformDirection(localOffset);
            targetPosition.y = cameraToFollow.position.y + yOffset;

            // Déplacer vers la position cible
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

            // Face the player with rotation offset
            Vector3 lookAtPos = cameraToFollow.position;
            lookAtPos.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - lookAtPos) * Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }

        bool IsMenuVisible()
        {
            if (viewCamera == null)
            {
                // Fallback: si on n'a pas de Camera, on ne "bloque" pas le follow (on considère hors champ)
                return false;
            }

            // Si on a un Renderer, on teste l'AABB contre le frustum (plus fiable que le seul pivot).
            var r = GetComponentInChildren<Renderer>();
            if (r != null)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(viewCamera);
                return GeometryUtility.TestPlanesAABB(planes, r.bounds);
            }

            // Sinon, on teste le pivot dans le viewport.
            Vector3 vp = viewCamera.WorldToViewportPoint(transform.position);
            if (vp.z <= 0f) return false; // derrière la caméra

            float m = viewportMargin;
            return vp.x >= 0f + m && vp.x <= 1f - m && vp.y >= 0f + m && vp.y <= 1f - m;
        }
    }
}