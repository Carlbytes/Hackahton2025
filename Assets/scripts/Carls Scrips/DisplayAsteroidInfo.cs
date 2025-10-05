using UnityEngine;
using System; // Required for Math.PI

[RequireComponent(typeof(AsteroidData))]
[RequireComponent(typeof(Collider))]
public class DisplayAsteroidInfo : MonoBehaviour
{
    private AsteroidData asteroidData;

    void Awake()
    {
        asteroidData = GetComponent<AsteroidData>();
    }

    private void OnMouseDown()
    {
        if (asteroidData != null && asteroidData.neoData != null)
        {
            NearEarthObject neo = asteroidData.neoData;

            // --- New Calculation ---
            double impactEnergyMegatons = CalculateImpactEnergy(neo);

            float avgDiameterMeters = (float)(neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0f;

            string info = $@"--- INFO: {neo.name} ---
ID: {neo.id}
Potentially Hazardous: {neo.is_potentially_hazardous_asteroid}
Sentry Object (Monitored): {neo.is_sentry_object}
Absolute Magnitude: {neo.absolute_magnitude_h}
--- Size & Trajectory ---
Avg. Diameter: {avgDiameterMeters:N0} meters
Relative Velocity: {float.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_hour):N0} km/h
--- ESTIMATED IMPACT ENERGY--- 
Equivalent to: {impactEnergyMegatons:N2} Megatons of TNT
--------------------";
            //Impact energy calculation is a rough estimate and assumes a stony asteroid with average density ignoring atmospheric effects.

            Debug.Log(info);
        }
    }

    /// <summary>
    /// Calculates the estimated kinetic energy of an asteroid on impact.
    /// </summary>
    /// <returns>The energy yield in Megatons of TNT.</returns>
    private double CalculateImpactEnergy(NearEarthObject neo)
    {
        // 1. Get average diameter in meters
        double avgDiameterMeters = (neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0;
        double radius = avgDiameterMeters / 2.0;

        // 2. Calculate volume assuming a sphere (V = 4/3 * pi * r^3)
        double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);

        // 3. Estimate mass (Mass = Volume * Density). Assume 3000 kg/m^3 for a stony asteroid.
        double density = 3000;
        double mass = volume * density; // Mass in kg

        // 4. Get velocity in meters per second
        double velocityKps = double.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_second);
        double velocityMps = velocityKps * 1000; // Velocity in m/s

        // 5. Calculate kinetic energy (E = 1/2 * m * v^2)
        double kineticEnergyJoules = 0.5 * mass * Math.Pow(velocityMps, 2);

        // 6. Convert joules to tons of TNT (1 ton TNT = 4.184e9 Joules)
        double energyTonsTNT = kineticEnergyJoules / 4.184e9;

        // 7. Convert to Megatons for easier reading
        double energyMegatonsTNT = energyTonsTNT / 1000000.0;

        return energyMegatonsTNT;
    }
}