using UnityEngine;

public class MeteorImpactCalculator : MonoBehaviour
{
    public struct MagnitudeResult
    {
        public float momentMagnitude;   // mW
        public float richterMagnitude;  // rS
    }
    public static MagnitudeResult EstimateEarthquakeMagnitude(float diameter, float velocity, float angleDegrees, float density = 3000f, float seismicEfficiency = 1e-4f)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        double kineticEnergy = (Mathf.PI / 12.0) * density *
                               Mathf.Pow(diameter, 3) * Mathf.Pow(velocity, 2);

        double seismicEnergy = seismicEfficiency * Mathf.Sin(angleRad) * kineticEnergy;

        double mW = (Mathf.Log10((float)seismicEnergy) - 4.8) / 1.5;

        double rS;
        if (mW <= 6.8)
            rS = mW;
        else
            rS = 6.8 + 0.67 * (mW - 6.8);

        return new MagnitudeResult
        {
            momentMagnitude = (float)mW,
            richterMagnitude = (float)rS
        };
    }

    // Example: test the function when game starts
    void Start()
    {
        float D = 50f;        // meters
        float V = 20000f;     // m/s
        float angle = 45f;    // degrees

        // Get the struct result
        MagnitudeResult result = EstimateEarthquakeMagnitude(D, V, angle);

        Debug.Log(result.momentMagnitude);
        Debug.Log(result.richterMagnitude);
    }
}
