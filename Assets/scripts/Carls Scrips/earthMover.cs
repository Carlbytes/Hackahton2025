using UnityEngine;

public class earthMover : MonoBehaviour
{
    [Header("Orbit")]
    public Transform center;                 
    public float orbitRadius = 5f;           
    public Vector3 orbitAxis = Vector3.up;   
    public float orbitDegreesPerSecond = 30f;
    public bool useCurrentOffsetAsStart = true;
    [Range(0f, 360f)] public float startAngleDegrees = 0f; 

    [Header("Self Rotation (Spin)")]
    public Vector3 selfAxis = Vector3.up;    
    public float selfDegreesPerSecond = 90f; 
    public bool spinInWorldSpace = false;    

    private Vector3 _offset;                 

    void Start()
    {
        if (center == null)
        {
            Debug.LogWarning($"[{nameof(earthMover)}] No center assigned. Disabling.");
            enabled = false;
            return;
        }

        if (orbitAxis != Vector3.zero) orbitAxis.Normalize();
        if (selfAxis != Vector3.zero) selfAxis.Normalize();

        if (useCurrentOffsetAsStart)
        {
            _offset = transform.position - center.position;
            if (_offset.sqrMagnitude < 0.0001f)
                _offset = Vector3.right * Mathf.Max(0.0001f, orbitRadius);
            else
                orbitRadius = _offset.magnitude; 
        }
        else
        {
           
            Vector3 basis = Vector3.Cross(orbitAxis, Vector3.up);
            if (basis.sqrMagnitude < 1e-6f) basis = Vector3.Cross(orbitAxis, Vector3.right);
            basis.Normalize();

            Quaternion startRot = Quaternion.AngleAxis(startAngleDegrees, orbitAxis);
            _offset = startRot * basis * Mathf.Max(0.0001f, orbitRadius);
            transform.position = center.position + _offset;
        }
    }

    void Update()
    {
        if (center == null) return;

        float orbitStep = orbitDegreesPerSecond * Time.deltaTime;
        Quaternion orbitRotation = Quaternion.AngleAxis(orbitStep, orbitAxis);
        _offset = orbitRotation * _offset;
        transform.position = center.position + _offset;

        float spinStep = selfDegreesPerSecond * Time.deltaTime;
        transform.Rotate(selfAxis, spinStep, spinInWorldSpace ? Space.World : Space.Self);
    }

    void OnValidate()
    {
        if (orbitAxis != Vector3.zero) orbitAxis = orbitAxis.normalized;
        if (selfAxis != Vector3.zero) selfAxis = selfAxis.normalized;

        if (orbitRadius < 0f) orbitRadius = 0f;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (center == null) return;

        Gizmos.color = Color.white;
        // Draw a simple circle to visualize the orbit in the Scene view
        const int segments = 96;
        Vector3 prev = Vector3.zero;
        Vector3 first = Vector3.zero;

        // Build a basis in the orbit plane
        Vector3 normal = orbitAxis.sqrMagnitude < 1e-6f ? Vector3.up : orbitAxis.normalized;
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 1e-6f) tangent = Vector3.Cross(normal, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent);

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float ang = t * Mathf.PI * 2f;
            Vector3 p = center.position + (tangent * Mathf.Cos(ang) + bitangent * Mathf.Sin(ang)) * orbitRadius;

            if (i == 0)
            {
                first = p;
            }
            else
            {
                Gizmos.DrawLine(prev, p);
            }
            prev = p;
        }
        // Close the loop
        Gizmos.DrawLine(prev, first);
    }
#endif
}