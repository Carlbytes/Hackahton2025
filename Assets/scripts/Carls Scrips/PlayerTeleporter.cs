using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public static PlayerTeleporter Instance;

    [Tooltip("How far from the asteroid the player should appear.")]
    [SerializeField] private float teleportOffset = 0.1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

  
    public void TeleportTo(Transform target)
    {
        if (target != null)
        {
         
            transform.position = target.position - (target.forward * teleportOffset);
            transform.LookAt(target);
            Debug.Log($"Teleporting to {target.name}.");
        }
    }
}