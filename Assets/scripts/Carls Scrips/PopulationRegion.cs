using UnityEngine;

[RequireComponent(typeof(Collider))] // Ensure a collider is always present
public class PopulationRegion : MonoBehaviour
{
    [Tooltip("The name of the region (e.g., USA, Europe).")]
    public string regionName;

    [Tooltip("Population of this region in millions.")]
    public float populationInMillions;

    private Collider regionCollider;

    void Awake()
    {
        // Get and store a reference to the collider on this object.
        regionCollider = GetComponent<Collider>();
    }

    public float CalculateCasualties(Vector3 impactPoint, float damageRadius)
    {
        if (regionCollider == null)
        {
            Debug.LogError($"Region '{regionName}' is missing a Collider component!");
            return 0;
        }

        // --- THIS IS THE CRITICAL CHANGE ---
        // Instead of using the object's pivot, find the closest point on the
        // surface of this region's collider to the impact point.
        Vector3 closestPointOnRegion = regionCollider.ClosestPoint(impactPoint);

        // Now, measure the distance from the impact to the edge of our region.
        float distanceToImpact = Vector3.Distance(closestPointOnRegion, impactPoint);

        // --- The rest of the logic remains the same ---
        if (distanceToImpact > damageRadius)
        {
            // If the closest point on our surface is still outside the blast, we are safe.
            return 0;
        }

        // The damage is highest at the edge of the region and decreases as it goes further in.
        // We simulate this by checking how much of the region is "covered" by the blast.
        float coverage = Mathf.Clamp01(1.0f - (distanceToImpact / damageRadius));
        float casualtiesInMillions = populationInMillions * coverage;

        Debug.Log($"Region '{regionName}' suffered {casualtiesInMillions:F2} million casualties.");

        return casualtiesInMillions;
    }
}