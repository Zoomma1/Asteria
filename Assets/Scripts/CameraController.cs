using UnityEngine;
using UnityEngine.InputSystem;

namespace Feature1
{
    public class CameraController : MonoBehaviour
{
    [Header("Paramètres de rotation")]
    [SerializeField] private float mouseSensitivity = 0.00000000001f;

    public Vector2 turn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // Curseur visible et libre
    }

    void Update()
    {
        if (Mouse.current == null) return;
        
        // Récupère le mouvement de la souris
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        turn.x += mouseDelta.x * mouseSensitivity;
        turn.y += mouseDelta.y * mouseSensitivity;
        
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
    
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
}

