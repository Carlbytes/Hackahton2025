using UnityEngine;

public class celestialRotation : MonoBehaviour
{

    [Header("Self Rotation (Spin)")]
    public Vector3 selfAxis = Vector3.up;    // Axis to spin around
    public float selfDegreesPerSecond = 90f; // Spin speed in degrees/sec
    public bool spinInWorldSpace = false;    // false = Space.Self, true = Space.World
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (selfAxis != Vector3.zero) selfAxis.Normalize();


    }

    // Update is called once per frame
    void Update()
    {

        float spinStep = selfDegreesPerSecond * Time.deltaTime;
        transform.Rotate(selfAxis, spinStep, spinInWorldSpace ? Space.World : Space.Self);

    }

    void OnValidate()
    {
        // Keep axes normalized in the inspector
        if (selfAxis != Vector3.zero) selfAxis = selfAxis.normalized;

    }
}
