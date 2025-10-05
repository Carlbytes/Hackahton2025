using UnityEngine;

public class PlanetDamageController : MonoBehaviour
{
    // A public reference to the Renderer of our planet.
    // We will drag our Earth object here in the Inspector.
    public Renderer planetRenderer;

    // The radius of our damage effect. We can change this in the Inspector.
    [Range(0, 10)]
    public float damageRadius = 2.5f;

    // An empty GameObject that we can move around to mark the impact point.
    public Transform impactMarker;

    // --- Shader Property IDs ---
    // It's a performance best practice to convert string names to IDs once.
    private static readonly int ImpactPositionID = Shader.PropertyToID("_Impact_Position");
    private static readonly int DamageRadiusID = Shader.PropertyToID("_Damage_Radius");

    void Update()
    {
        // First, ensure both the renderer and marker are set to avoid errors.
        if (planetRenderer == null || impactMarker == null)
        {
            return; // Exit the function if anything is missing.
            
        }

        // Get the planet's material instance for this object.
        Material planetMaterial = planetRenderer.material;

        // Send our data to the material's shader every frame.
        // The shader can now access "_Impact_Position" and "_Damage_Radius".
        planetMaterial.SetVector(ImpactPositionID, impactMarker.position);
        planetMaterial.SetFloat(DamageRadiusID, damageRadius);
    }
}