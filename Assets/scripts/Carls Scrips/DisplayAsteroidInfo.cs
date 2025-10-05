using UnityEngine;
using System; 

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

            Debug.Log(info);
        }
    }

   
    private double CalculateImpactEnergy(NearEarthObject neo)
    {
        double avgDiameterMeters = (neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0;
        double radius = avgDiameterMeters / 2.0;

        double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);

        double density = 3000;
        double mass = volume * density; 

        double velocityKps = double.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_second);
        double velocityMps = velocityKps * 1000; // Velocity in m/s

        double kineticEnergyJoules = 0.5 * mass * Math.Pow(velocityMps, 2);

        double energyTonsTNT = kineticEnergyJoules / 4.184e9;

        double energyMegatonsTNT = energyTonsTNT / 1000000.0;

        return energyMegatonsTNT;
    }
}