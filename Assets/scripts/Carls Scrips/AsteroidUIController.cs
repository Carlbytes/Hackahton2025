using UnityEngine;
using TMPro; // Required for TextMeshPro

public class AsteroidUIController : MonoBehaviour
{
    // --- UI Field References ---
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI diameterText;
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI hazardousText;
    // Add references for any other text fields you have

    /// <summary>
    /// A public method to receive asteroid data and update the UI.
    /// This will be called by the AsteroidManager.
    /// </summary>
    public void UpdateUI(NearEarthObject neo)
    {
        if (neo == null) return;

        // --- Update Text Fields ---
        nameText.text = neo.name;

        float avgDiameterMeters = (float)(neo.estimated_diameter.meters.estimated_diameter_min + neo.estimated_diameter.meters.estimated_diameter_max) / 2.0f;
        diameterText.text = $"Avg. Diameter: {avgDiameterMeters:N0} m";

        float velocityKmh = float.Parse(neo.close_approach_data[0].relative_velocity.kilometers_per_hour);
        velocityText.text = $"Velocity: {velocityKmh:N0} km/h";

        hazardousText.text = neo.is_potentially_hazardous_asteroid ? "Potentially Hazardous" : "Not Hazardous";
        hazardousText.color = neo.is_potentially_hazardous_asteroid ? Color.red : Color.green;
    }
}