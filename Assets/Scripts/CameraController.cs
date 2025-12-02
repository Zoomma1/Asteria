using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 200f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private float smoothSpeed = 10f;

        private float yaw;
        private float pitch;
        private Quaternion targetRotation;
        private InputAction lookAction;
        private InputAction attackAction;

        private void Awake()
        {
            lookAction = new InputAction("Look", binding: "<Pointer>/delta");
            attackAction = new InputAction("Attack", binding: "<Mouse>/leftButton");
            lookAction.Enable();
            attackAction.Enable();
        }

        private void Start()
        {
            Vector3 euler = transform.localEulerAngles;
            yaw = euler.y;
            pitch = euler.x;
            targetRotation = transform.localRotation;
        }

        private void Update()
        {
            if (attackAction.IsPressed())
            {
                Vector2 delta = lookAction.ReadValue<Vector2>();
                yaw += delta.x * sensitivity * Time.deltaTime;
                pitch += (invertY ? delta.y : -delta.y) * sensitivity * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                targetRotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            // Lissage de la rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            lookAction.Disable();
            attackAction.Disable();
        }
    }
}