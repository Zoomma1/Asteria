using UnityEngine;

namespace Menu
{
    public class LazyFollow_Backup : MonoBehaviour
    {
        public Transform cameraToFollow;
        
        [Header("Position Settings")]
        public float distance = 3.0f;      // Distance in front of the player (Z axis)
        public float xOffset = 0.0f;       // Lateral offset (left/right)
        public float yOffset = -0.5f;      // Vertical offset (up/down)
        
        [Header("Rotation Settings")]
        public Vector3 targetOffsetRotation = Vector3.zero; // Additional rotation offset (in degrees)
        
        [Header("Behavior Settings")]
        public float smoothSpeed = 2.0f;   // Smooth speed (between 1 and 5)
        public float angleThreshold = 40.0f;   // Only moves if head turns more than 40°
        
        // Hidden parameters - configurable in code only
        private bool followInLocalSpace = true;           // Follow in local space
        private bool applyTargetInLocalSpace = true;      // Apply target in local space
        
        // General Follow Parameters (hidden)
        private bool enablePositionFollow = true;
        private bool enableRotationFollow = true;
        
        // Position Follow Parameters (hidden)
        private bool useLocalOffset = true;               // If true, offsets are relative to camera
        private bool lockHeight = true;                   // If true, keeps camera height + yOffset
        
        // Rotation Follow Parameters (hidden)
        private bool lockRotationX = false;
        private bool lockRotationY = false;
        private bool lockRotationZ = false;

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
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - lookAtPos) * Quaternion.Euler(targetOffsetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}

