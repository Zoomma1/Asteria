using UnityEngine;

namespace Menu
{
    public class LazyFollow : MonoBehaviour
    {
        public Transform cameraToFollow;
        
        [Header("Réglages de Position")]
        public float distance = 3.0f;      // 3 mètres est une bonne distance VR
        public float smoothSpeed = 2.0f;   // Vitesse fluide (entre 1 et 5)
        public float angleSeuil = 40.0f;   // Ne bouge que si on tourne la tête de 40°
        
        [Header("Réglages de Hauteur")]
        public float yOffset = -0.5f;      // Pour baisser le menu un peu sous les yeux

        void Update()
        {
            if (cameraToFollow == null) return;

            // 1. Calcul de la position cible (devant la caméra)
            Vector3 targetPosition = cameraToFollow.position + (cameraToFollow.forward * distance);
            
            // On fixe la hauteur pour ne pas avoir le mal de mer
            targetPosition.y = cameraToFollow.position.y + yOffset; 

            // 2. Calcul de l'angle horizontal
            Vector3 directionToMenu = transform.position - cameraToFollow.position;
            directionToMenu.y = 0; // On ignore la hauteur pour l'angle
            Vector3 cameraForward = cameraToFollow.forward;
            cameraForward.y = 0;

            float angle = Vector3.Angle(cameraForward, directionToMenu);

            // 3. Suivi souple si on dépasse le seuil
            if (angle > angleSeuil)
            {
                // Utilise une vitesse plus naturelle (ex: 2.0f)
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }

            // 4. Rotation pour toujours faire face au joueur
            Vector3 lookAtPos = cameraToFollow.position;
            lookAtPos.y = transform.position.y; 
            
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - lookAtPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}