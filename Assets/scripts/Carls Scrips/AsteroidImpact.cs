using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class RegionData
{
    public string regionName;
    public float populationInMillions;
    public float radiusKm;
}

[RequireComponent(typeof(AsteroidData))]
public class AsteroidImpact : MonoBehaviour
{
    public List<RegionData> regions = new List<RegionData>
    {
        new RegionData { regionName = "North America", populationInMillions = 42.1f, radiusKm = 1500f },
        new RegionData { regionName = "United States of America", populationInMillions = 330.8f, radiusKm = 2000f },
        new RegionData { regionName = "Central America", populationInMillions = 183.4f, radiusKm = 1000f },
        new RegionData { regionName = "South America North", populationInMillions = 294.8f, radiusKm = 2200f },
        new RegionData { regionName = "South(South) America", populationInMillions = 123.2f, radiusKm = 1800f },
        new RegionData { regionName = "Europe North", populationInMillions = 109.5f, radiusKm = 1200f },
        new RegionData { regionName = "Europe West", populationInMillions = 199.8f, radiusKm = 1000f },
        new RegionData { regionName = "Europe East", populationInMillions = 140.2f, radiusKm = 1500f },
        new RegionData { regionName = "Russian West", populationInMillions = 114.8f, radiusKm = 2500f },
        new RegionData { regionName = "Russia East", populationInMillions = 28.7f, radiusKm = 3000f },
        new RegionData { regionName = "Saudai & Nearby", populationInMillions = 392.3f, radiusKm = 2000f },
        new RegionData { regionName = "Pakisan & Surrounding", populationInMillions = 378.4f, radiusKm = 1500f },
        new RegionData { regionName = "India & Surrounding", populationInMillions = 1746f, radiusKm = 1800f },
        new RegionData { regionName = "Eastern Asia", populationInMillions = 1842f, radiusKm = 2500f },
        new RegionData { regionName = "Southern Asia", populationInMillions = 458.1f, radiusKm = 1500f },
        new RegionData { regionName = "Oceanic", populationInMillions = 33.4f, radiusKm = 2000f },
        new RegionData { regionName = "Northern Africa", populationInMillions = 642.9f, radiusKm = 2500f },
        new RegionData { regionName = "Eastern Africa", populationInMillions = 642.9f, radiusKm = 2000f },
        new RegionData { regionName = "Southern Africa", populationInMillions = 311.2f, radiusKm = 1500f },
        new RegionData { regionName = "Antartica", populationInMillions = 0f, radiusKm = 2000f }
    };

    [SerializeField] private GameObject shockwavePrefab;
    private AsteroidData asteroidData;
    private bool hasImpacted = false;

    void Awake()
    {
        asteroidData = GetComponent<AsteroidData>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) return;
        hasImpacted = true;

        if (asteroidData.neoData == null || asteroidData.neoData.close_approach_data == null || asteroidData.neoData.close_approach_data.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 impactPoint = collision.contacts[0].point;
        double megatons = CalculateImpactEnergy(asteroidData.neoData, 3000, 45);
        float damageRadiusMultiplier = 700f;
        float damageRadiusKm = Mathf.Log10((float)megatons + 1) * damageRadiusMultiplier;

        StringBuilder report = new StringBuilder();
        report.AppendLine($"--- HYPOTHETICAL IMPACT REPORT ({asteroidData.neoData.name}) ---");
        report.AppendLine($"Energy: {megatons:F2} Megatons. Est. Damage Radius: {damageRadiusKm:F2} km.");
        report.AppendLine("--- Casualty Estimates by Region ---");

        float totalCasualties = 0f;

        foreach (RegionData region in regions)
        {
            float casualties = 0f;
            if (damageRadiusKm > 0 && region.radiusKm > 0)
            {
                float areaRatio = (damageRadiusKm * damageRadiusKm) / (region.radiusKm * region.radiusKm);
                casualties = region.populationInMillions * Mathf.Clamp01(areaRatio);
            }

            // Add the formatted string to the report
            report.AppendLine($"{region.regionName}: {FormatPopulation(casualties)} / {FormatPopulation(region.populationInMillions)}");

            // Add to the global total
            totalCasualties += casualties;
        }

        // --- Add the Global Death Count if the impact is large enough ---
        if (megatons > 1000) // Threshold for a globally significant event
        {
            report.AppendLine("--------------------");
            report.AppendLine($"GLOBAL DEATHCOUNT: {FormatPopulation(totalCasualties)}");
        }

        report.AppendLine("--------------------");
        // The corrected version

        ImpactReportUI reportUI = ImpactReportUI.Instance;

        if (reportUI == null)
        {
            reportUI = FindObjectOfType<ImpactReportUI>(true); // 'true' allows finding inactive objects
        }

        // 3. Now, use the UI if we found it, otherwise fall back to the console.
        if (reportUI != null)
        {
            reportUI.DisplayReport(report.ToString());
        }
        else
        {
            Debug.LogWarning("Could not find ImpactReportUI in the scene. Falling back to console log.");
            Debug.Log(report.ToString());
        }
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, impactPoint, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Formats a number (in millions) into a readable string, switching to billions if needed.
    /// </summary>
    private string FormatPopulation(float valueInMillions)
    {
        if (valueInMillions >= 1000)
        {
            return $"{(valueInMillions / 1000):F2} Billion";
        }
        return $"{valueInMillions:F2} Million";
    }

    private double CalculateImpactEnergy(NearEarthObject neo, double density, double angleDegrees)
    {
        double avgDiameterMeters = (neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0;
        double radius = avgDiameterMeters / 2.0;
        double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
        double mass = volume * density;
        double velocityKps = double.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_second);
        double velocityMps = velocityKps * 1000;
        double initialKineticEnergy = 0.5 * mass * Math.Pow(velocityMps, 2);
        double angleRadians = angleDegrees * (Math.PI / 180.0);
        double atmosphericMultiplier = Math.Sin(angleRadians);
        double finalKineticEnergy = initialKineticEnergy * atmosphericMultiplier;
        double energyTonsTNT = finalKineticEnergy / 4.184e9;
        return energyTonsTNT / 1000000.0;
    }
}