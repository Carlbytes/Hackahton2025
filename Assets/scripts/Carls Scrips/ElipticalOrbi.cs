using UnityEngine;

public class ElipticalOrbi : MonoBehaviour
{
    [Header("Centers & Axes")]
    public Transform center;                        
    [Tooltip("Normal of the orbit plane (world-space).")]
    public Vector3 orbitPlaneNormal = Vector3.up;
    [Tooltip("Direction of periapsis within the orbit plane. If zero, a stable perpendicular is chosen.")]
    public Vector3 periapsisDirection = Vector3.forward;
    [Tooltip("If true, 'center' is the ellipse FOCUS (realistic). If false, 'center' is ellipse CENTER.")]
    public bool centerIsFocus = true;

    [Header("Ellipse Shape")]
    [Tooltip("Semi-major axis length (a).")]
    public float semiMajorAxis = 5f;              
    [Range(0f, 0.99f)]
    [Tooltip("Eccentricity (e). 0 = circle, <1 = ellipse.")]
    public float eccentricity = 0.3f;              

    [Header("Orbit Timing")]
    [Tooltip("Complete-orbit time (seconds) for Keplerian/ConstantAngle modes.")]
    public float orbitalPeriodSeconds = 10f;
    [Tooltip("Initial position: 0 = periapsis, 180 = apoapsis (true anomaly, degrees).")]
    [Range(0f, 360f)]
    public float startTrueAnomalyDeg = 0f;

    
    public enum SpeedMode { Keplerian, ConstantAngle, ConstantLinear }
    [Header("Speed Mode")]
    public SpeedMode speedMode = SpeedMode.Keplerian;
    [Tooltip("Linear speed along the path (units/sec) when using ConstantLinear.")]
    public float linearSpeed = 3f;

    [Header("Self Rotation (Spin)")]
    public Vector3 selfAxis = Vector3.up;
    public float selfDegreesPerSecond = 90f;
    public bool spinInWorldSpace = false;

    // === Internal state ===
    private Vector3 nrm, majorDir, minorDir; 
    private float a, e, b, c;                
    private float meanMotion;               
    private float meanAnomaly;              
    private float trueAnomaly;               

    // Arc-length LUT for ConstantLinear mode
    private const int LUT_SAMPLES = 1024;
    private float[] sLUT;           
    private float totalArcLength;   
    private float sAlong;           

    void Start()
    {
        if (center == null)
        {
            Debug.LogWarning($"[{nameof(ElipticalOrbi)}] No center assigned. Disabling.");
            enabled = false;
            return;
        }

        a = Mathf.Max(0.0001f, semiMajorAxis);
        e = Mathf.Clamp(eccentricity, 0f, 0.99f);
        b = a * Mathf.Sqrt(1f - e * e);
        c = a * e;

        nrm = orbitPlaneNormal.sqrMagnitude < 1e-8f ? Vector3.up : orbitPlaneNormal.normalized;

        Vector3 basis = periapsisDirection;
        if (basis.sqrMagnitude < 1e-8f)
        {
            basis = Vector3.Cross(nrm, Vector3.up);
            if (basis.sqrMagnitude < 1e-8f) basis = Vector3.Cross(nrm, Vector3.right);
        }
        majorDir = Vector3.ProjectOnPlane(basis.normalized, nrm).normalized;
        if (majorDir.sqrMagnitude < 1e-8f) majorDir = Vector3.Cross(nrm, Vector3.right).normalized;
        minorDir = Vector3.Cross(nrm, majorDir).normalized;

        meanMotion = Mathf.PI * 2f / Mathf.Max(0.0001f, orbitalPeriodSeconds);
        trueAnomaly = Mathf.Deg2Rad * startTrueAnomalyDeg;

        if (speedMode == SpeedMode.Keplerian)
        {
            float E0 = TrueToEccentric(trueAnomaly, e);
            meanAnomaly = NormalizeAngle(E0 - e * Mathf.Sin(E0));
        }

        BuildArcLengthLUT();
        if (speedMode == SpeedMode.ConstantLinear)
        {
            float nu0 = Mathf.Deg2Rad * startTrueAnomalyDeg;
            sAlong = ArcLengthFromNu(nu0);
        }

        ApplyInitialPosition();
    }

    void Update()
    {
        if (center == null) return;

        float dt = Time.deltaTime;

        switch (speedMode)
        {
            case SpeedMode.Keplerian:
                {
                    meanAnomaly = NormalizeAngle(meanAnomaly + meanMotion * dt);
                    float E = SolveKeplersEquation(meanAnomaly, e);
                    float nu = EccentricToTrue(E, e);
                    SetPositionByTrueAnomaly(nu);
                    break;
                }
            case SpeedMode.ConstantAngle:
                {
                    trueAnomaly = NormalizeAngle(trueAnomaly + meanMotion * dt);
                    SetPositionByTrueAnomaly(trueAnomaly);
                    break;
                }
            case SpeedMode.ConstantLinear:
                {
                    sAlong += Mathf.Max(0f, linearSpeed) * dt;
                    if (sAlong >= totalArcLength) sAlong -= totalArcLength;
                    float nu = NuFromArcLength(sAlong);
                    SetPositionByTrueAnomaly(nu);
                    break;
                }
        }

        // Self spin
        float spinStep = selfDegreesPerSecond * dt;
        transform.Rotate(selfAxis, spinStep, spinInWorldSpace ? Space.World : Space.Self);
    }

    // === Placement helpers ===
    private void ApplyInitialPosition()
    {
        switch (speedMode)
        {
            case SpeedMode.Keplerian:
                {
                    float E = SolveKeplersEquation(meanAnomaly, e);
                    float nu = EccentricToTrue(E, e);
                    SetPositionByTrueAnomaly(nu);
                    break;
                }
            case SpeedMode.ConstantAngle:
                {
                    SetPositionByTrueAnomaly(trueAnomaly);
                    break;
                }
            case SpeedMode.ConstantLinear:
                {
                    SetPositionByTrueAnomaly(NuFromArcLength(sAlong));
                    break;
                }
        }
    }

    private void SetPositionByTrueAnomaly(float nu)
    {
        // Radius in polar form (focus at origin): r = a(1 - e^2) / (1 + e cos ν)
        float r = a * (1f - e * e) / (1f + e * Mathf.Cos(nu));

        // Position in orbit plane relative to FOCUS
        Vector3 pFocus = r * (Mathf.Cos(nu) * majorDir + Mathf.Sin(nu) * minorDir);

        if (centerIsFocus)
        {
            transform.position = center.position + pFocus;
        }
        else
        {
            // If 'center' is the ellipse center, convert from focus frame to center frame by -c along major axis
            transform.position = center.position + (pFocus - c * majorDir);
        }
    }

    // === Math helpers ===
    private static float NormalizeAngle(float angleRad)
    {
        float twoPi = Mathf.PI * 2f;
        angleRad %= twoPi;
        if (angleRad < 0f) angleRad += twoPi;
        return angleRad;
    }

    private static float SolveKeplersEquation(float M, float e)
    {
        // Newton-Raphson for E - e sinE = M
        float E = M; // good initial guess for small/moderate e
        for (int i = 0; i < 6; i++)
        {
            float f = E - e * Mathf.Sin(E) - M;
            float fp = 1f - e * Mathf.Cos(E);
            E -= f / Mathf.Max(1e-6f, fp);
        }
        return E;
    }

    private static float EccentricToTrue(float E, float e)
    {
        float cosE = Mathf.Cos(E);
        float sinE = Mathf.Sin(E);
        float denom = 1f - e * cosE;
        float sqrt = Mathf.Sqrt(Mathf.Max(0f, 1f - e * e));
        float cosNu = (cosE - e) / Mathf.Max(1e-6f, denom);
        float sinNu = (sqrt * sinE) / Mathf.Max(1e-6f, denom);
        return Mathf.Atan2(sinNu, cosNu);
    }

    private static float TrueToEccentric(float nu, float e)
    {
        float cosNu = Mathf.Cos(nu);
        float sinNu = Mathf.Sin(nu);
        float sqrt = Mathf.Sqrt(Mathf.Max(0f, 1f - e * e));
        float E = Mathf.Atan2(sinNu * sqrt, e + cosNu);
        if (E < 0f) E += Mathf.PI * 2f;
        return E;
    }

    // === Arc-length LUT (ConstantLinear) ===
    private Vector3 PointAtNu(float nu)
    {
        // Focus-based polar form, then shift if drawing with ellipse center
        float r = a * (1f - e * e) / (1f + e * Mathf.Cos(nu));
        Vector3 pFocus = r * (Mathf.Cos(nu) * majorDir + Mathf.Sin(nu) * minorDir);

        if (centerIsFocus)
            return center.position + pFocus;

        return center.position + (pFocus - c * majorDir);
    }

    private void BuildArcLengthLUT()
    {
        sLUT = new float[LUT_SAMPLES + 1];
        sLUT[0] = 0f;

        Vector3 prev = PointAtNu(0f);
        float accum = 0f;

        for (int i = 1; i <= LUT_SAMPLES; i++)
        {
            float t = (i / (float)LUT_SAMPLES) * (Mathf.PI * 2f);
            Vector3 p = PointAtNu(t);
            accum += Vector3.Distance(prev, p);
            sLUT[i] = accum;
            prev = p;
        }
        totalArcLength = Mathf.Max(accum, 1e-6f);
    }

    private float ArcLengthFromNu(float nu)
    {
        // Map parameter t = nu (0..2π) to LUT index directly
        float t = NormalizeAngle(nu);
        int idx = Mathf.Clamp(Mathf.RoundToInt((t / (Mathf.PI * 2f)) * LUT_SAMPLES), 0, LUT_SAMPLES);
        return sLUT[idx];
    }

    private float NuFromArcLength(float s)
    {
        // Binary search cumulative table, then linear interpolate the parameter
        s = Mathf.Repeat(s, totalArcLength);

        int lo = 0, hi = LUT_SAMPLES;
        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (sLUT[mid] < s) lo = mid + 1;
            else hi = mid;
        }
        int i1 = Mathf.Clamp(lo, 1, LUT_SAMPLES);
        int i0 = i1 - 1;

        float s0 = sLUT[i0];
        float s1 = sLUT[i1];
        float u = (s1 > s0) ? (s - s0) / (s1 - s0) : 0f;

        float t0 = (i0 / (float)LUT_SAMPLES) * (Mathf.PI * 2f);
        float t1 = (i1 / (float)LUT_SAMPLES) * (Mathf.PI * 2f);
        return Mathf.Lerp(t0, t1, u);
    }

    void OnValidate()
    {
        if (orbitPlaneNormal != Vector3.zero) orbitPlaneNormal = orbitPlaneNormal.normalized;
        if (periapsisDirection != Vector3.zero) periapsisDirection = periapsisDirection.normalized;
        if (selfAxis != Vector3.zero) selfAxis = selfAxis.normalized;

        semiMajorAxis = Mathf.Max(0f, semiMajorAxis);
        eccentricity = Mathf.Clamp(eccentricity, 0f, 0.99f);
        orbitalPeriodSeconds = Mathf.Max(0.0001f, orbitalPeriodSeconds);
        linearSpeed = Mathf.Max(0f, linearSpeed);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (center == null) return;

        // Recompute quick basis for gizmo (safe for edit-time tweaks)
        Vector3 n = orbitPlaneNormal.sqrMagnitude < 1e-8f ? Vector3.up : orbitPlaneNormal.normalized;
        Vector3 basis = periapsisDirection.sqrMagnitude < 1e-8f
            ? Vector3.Cross(n, Vector3.up)
            : Vector3.ProjectOnPlane(periapsisDirection, n);
        if (basis.sqrMagnitude < 1e-8f) basis = Vector3.Cross(n, Vector3.right);
        Vector3 U = basis.normalized;                 // major axis dir
        Vector3 V = Vector3.Cross(n, U).normalized;   // minor axis dir

        float aG = Mathf.Max(0.0001f, semiMajorAxis);
        float eG = Mathf.Clamp(eccentricity, 0f, 0.99f);
        float cG = aG * eG;

        Gizmos.color = Color.white;
        const int segs = 160;
        Vector3 prev = Vector3.zero;
        bool hasPrev = false;

        for (int i = 0; i <= segs; i++)
        {
            float t = (i / (float)segs) * (Mathf.PI * 2f);

            // Draw via the same world-space PointAtNu used by movement,
            // but with a temporary basis matching current inspector values.
            float r = aG * (1f - eG * eG) / (1f + eG * Mathf.Cos(t));
            Vector3 pFocus = r * (Mathf.Cos(t) * U + Mathf.Sin(t) * V);

            Vector3 origin;
            if (centerIsFocus) origin = center.position + pFocus;
            else origin = center.position + (pFocus - cG * U);

            if (hasPrev) Gizmos.DrawLine(prev, origin);
            prev = origin;
            hasPrev = true;
        }
    }
#endif
}