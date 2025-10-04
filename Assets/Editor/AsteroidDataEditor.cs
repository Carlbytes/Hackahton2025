using UnityEngine;
using UnityEditor;
using System; // Required for Math.PI and Math.Pow

// This tells Unity that this script is a custom editor for the AsteroidData component.
[CustomEditor(typeof(AsteroidData))]
public class AsteroidDataEditor : Editor
{
    // This method overrides how the Inspector is drawn.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AsteroidData data = (AsteroidData)target;

        if (GUILayout.Button("Log Asteroid Data to Console"))
        {
            if (data.neoData != null)
            {
                NearEarthObject neo = data.neoData;

                // --- Perform the impact energy calculation ---
                double impactEnergyMegatons = CalculateImpactEnergy(neo);

                float avgDiameterMeters = (float)(neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0f;

                string info = $@"--- INSPECTOR INFO: {neo.name} ---
ID: {neo.id}
Potentially Hazardous: {neo.is_potentially_hazardous_asteroid}
Sentry Object (Monitored): {neo.is_sentry_object}
Absolute Magnitude: {neo.absolute_magnitude_h}
--- Size & Trajectory ---
Avg. Diameter: {avgDiameterMeters:N0} meters
Closest Approach: {neo.close_approach_data[0].close_approach_date_full}
Relative Velocity: {float.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_hour):N0} km/h
Miss Distance: {float.Parse(neo.close_approach_data[0].miss_distance.kilometers):N0} km
--- ESTIMATED IMPACT ENERGY ---
Equivalent to: {impactEnergyMegatons:N2} Megatons of TNT
--- Orbital Data ---
Semi-Major Axis: {neo.orbital_data.semi_major_axis} AU
Eccentricity: {neo.orbital_data.eccentricity}
Inclination: {neo.orbital_data.inclination} deg
Orbital Period: {float.Parse(neo.orbital_data.orbital_period_in_days):N0} days
--------------------";

                Debug.Log(info);
            }
        }
    }

    /// <summary>
    /// Calculates the estimated kinetic energy of an asteroid on impact.
    /// </summary>
    private double CalculateImpactEnergy(NearEarthObject neo)
    {
        // 1. Get average diameter in meters
        double avgDiameterMeters = (neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0;
        double radius = avgDiameterMeters / 2.0;

        // 2. Calculate volume assuming a sphere (V = 4/3 * pi * r^3)
        double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);

        // 3. Estimate mass (Mass = Volume * Density). Assume 3000 kg/m^3.
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