using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AsteroidButtonController : MonoBehaviour
{
    // --- Data Fields ---
    private NearEarthObject neoData;
    private Transform asteroidTransform;
    private TextMeshProUGUI buttonText;

    private void Awake()
    {
        // Get the button component and add a listener for when it's clicked
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// This public method is called by the AsteroidManager to set up the button.
    /// </summary>
    public void Initialize(NearEarthObject data, Transform target)
    {
        neoData = data;
        asteroidTransform = target;
        buttonText.text = neoData.name; // Set the button's text to the asteroid name
    }

    /// <summary>
    /// Called when the player clicks this button.
    /// </summary>
    private void OnButtonClicked()
    {
        // 1. Teleport the player
        PlayerTeleporter.Instance.TeleportTo(asteroidTransform);

        // 2. Display the stats (using the same logging method as before)
        LogAsteroidInfo();
    }

    private void LogAsteroidInfo()
    {
        // You could also have this method update a central UI panel instead of logging
        double impactAngleDegrees = 45.0;
        double stonyImpactMegatons = CalculateImpactEnergy(neoData, 3000, impactAngleDegrees);
        double ironImpactMegatons = CalculateImpactEnergy(neoData, 8000, impactAngleDegrees);

        string info = $@"--- TELEPORT INFO: {neoData.name} ---
Avg. Diameter: {(neoData.estimated_diameter.meters.estimated_diameter_min + neoData.estimated_diameter.meters.estimated_diameter_max) / 2.0f:N0} meters
--- ESTIMATED IMPACT ENERGY (assuming {impactAngleDegrees}° entry angle) ---
If Stony (3000 kg/m³): {stonyImpactMegatons:N2} Megatons of TNT
If Iron (8000 kg/m³): {ironImpactMegatons:N2} Megatons of TNT
--------------------";

        Debug.Log(info);
    }

    // (Include the CalculateImpactEnergy method from your other scripts here)
    private double CalculateImpactEnergy(NearEarthObject neo, double density, double angleDegrees)
    {
        // ... paste the full calculation method here ...
        double avgDiameterMeters = (neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0;
        double radius = avgDiameterMeters / 2.0;
        double volume = (4.0 / 3.0) * System.Math.PI * System.Math.Pow(radius, 3);
        double mass = volume * density;
        double velocityKps = double.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_second);
        double velocityMps = velocityKps * 1000;
        double initialKineticEnergy = 0.5 * mass * System.Math.Pow(velocityMps, 2);
        double angleRadians = angleDegrees * (System.Math.PI / 180.0);
        double atmosphericMultiplier = System.Math.Sin(angleRadians);
        double finalKineticEnergy = initialKineticEnergy * atmosphericMultiplier;
        double energyTonsTNT = finalKineticEnergy / 4.184e9;
        return energyTonsTNT / 1000000.0;
    }
}