using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    // A 'static instance' makes it easy to access this script from anywhere.
    public static PlayerTeleporter Instance;

    [Tooltip("How far from the asteroid the player should appear.")]
    [SerializeField] private float teleportOffset = 0.1f;

    void Awake()
    {
        // Set up the static instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves the player to a position near the target asteroid.
    /// </summary>
    public void TeleportTo(Transform target)
    {
        if (target != null)
        {
            // Position the player 'teleportOffset' units away from the asteroid
            // and make them look at it.
            transform.position = target.position - (target.forward * teleportOffset);
            transform.LookAt(target);
            Debug.Log($"Teleporting to {target.name}.");
        }
    }
}