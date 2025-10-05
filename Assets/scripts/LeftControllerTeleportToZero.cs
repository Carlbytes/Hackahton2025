using UnityEngine;
using UnityEngine.InputSystem;

public class LeftControllerTeleporter : MonoBehaviour
{
    [Tooltip("The Input Action associated with the Left Controller's 'X' button (Primary Button).")]
    public InputActionProperty teleportAction;

    [Tooltip("Drag your XR Origin/XR Rig GameObject here.")]
    public Transform xrRigTransform;

    [Tooltip("The target world position (0, 0, -15 is the default).")]
    public Vector3 targetPosition = new Vector3(0f, 0f, -15f);

    private void OnEnable()
    {
        // Enable the input action when this script is enabled
        teleportAction.action.Enable();
        // Subscribe to the 'performed' phase of the action (when the button is pressed and released)
        teleportAction.action.performed += OnTeleportPerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe and disable the input action when this script is disabled
        teleportAction.action.performed -= OnTeleportPerformed;
        teleportAction.action.Disable();
    }

    private void OnTeleportPerformed(InputAction.CallbackContext context)
    {
        if (xrRigTransform != null)
        {
            // Teleport the player (XR Rig) to the target position
            xrRigTransform.position = targetPosition;
            Debug.Log($"Teleported XR Rig to {targetPosition}");
        }
        else
        {
            Debug.LogWarning("XR Rig Transform is not assigned! Please assign the XR Origin.");
        }
    }
}