using UnityEngine;

namespace Menu
{
    public class LazyFollow : MonoBehaviour
    {
        public Transform cameraToFollow;
        public float distance = 3.0f;
        public float xOffset = 0.0f;
        public float yOffset = 4f;
        public Vector3 rotationOffset = Vector3.zero;
        public float smoothSpeed = 2.0f;
        public float angleThreshold = 40.0f;

        void Update()
        {
            if (cameraToFollow == null) return;

            // Calculate target position
            Vector3 localOffset = new Vector3(xOffset, yOffset, distance);
            Vector3 targetPosition = cameraToFollow.position + cameraToFollow.TransformDirection(localOffset);
            targetPosition.y = cameraToFollow.position.y + yOffset;

            // Calculate angle
            Vector3 directionToMenu = transform.position - cameraToFollow.position;
            directionToMenu.y = 0;
            Vector3 cameraForward = cameraToFollow.forward;
            cameraForward.y = 0;
            float angle = Vector3.Angle(cameraForward, directionToMenu);

            // Move if threshold exceeded
            if (angle > angleThreshold)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }

            // Face the player with rotation offset
            Vector3 lookAtPos = cameraToFollow.position;
            lookAtPos.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - lookAtPos) * Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}