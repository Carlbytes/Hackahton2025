using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class ReturnToOrigin : MonoBehaviour
{
    [Tooltip("The Transform of the XR Origin (or player rig) that you want to reset.")]
    public Transform xrOriginTransform;

    [Header("Controller Input")]
    [Tooltip("The Input Action that will trigger the teleport (e.g., X button on left controller).")]
    public InputActionProperty teleportAction; // This will hold our button reference

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        if (xrOriginTransform == null)
        {
            Debug.LogError("XR Origin Transform is not assigned! Please assign it in the Inspector.", this);
            this.enabled = false;
            return;
        }

        initialPosition = xrOriginTransform.position;
        initialRotation = xrOriginTransform.rotation;
    }
    
    // Subscribe to the input action event when this object is enabled.
    private void OnEnable()
    {
        teleportAction.action.Enable();
        teleportAction.action.performed += OnTeleportPerformed;
    }

    // Unsubscribe from the event when this object is disabled to prevent errors.
    private void OnDisable()
    {
        teleportAction.action.performed -= OnTeleportPerformed;
        teleportAction.action.Disable();
    }

    // This function is called ONLY when the assigned button is pressed.
    private void OnTeleportPerformed(InputAction.CallbackContext context)
    {
        TeleportToOrigin();
    }

    // The core teleport logic remains the same.
    public void TeleportToOrigin()
    {
        if (xrOriginTransform != null)
        {
            xrOriginTransform.position = initialPosition;
            xrOriginTransform.rotation = initialRotation;
            Debug.Log("Player returned to origin via controller button.");
        }
    }
}