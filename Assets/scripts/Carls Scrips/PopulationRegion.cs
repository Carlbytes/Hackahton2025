using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class PopulationRegion : MonoBehaviour
{
    [Tooltip("The name of the region (e.g., USA, Europe).")]
    public string regionName;

    [Tooltip("Population of this region in millions.")]
    public float populationInMillions;

    private Collider regionCollider;

    void Awake()
    {
        regionCollider = GetComponent<Collider>();
    }

    public float CalculateCasualties(Vector3 impactPoint, float damageRadius)
    {
        if (regionCollider == null)
        {
            Debug.LogError($"Region '{regionName}' is missing a Collider component!");
            return 0;
        }

    
        Vector3 closestPointOnRegion = regionCollider.ClosestPoint(impactPoint);

        float distanceToImpact = Vector3.Distance(closestPointOnRegion, impactPoint);

        if (distanceToImpact > damageRadius)
        {
            return 0;
        }

  
        float coverage = Mathf.Clamp01(1.0f - (distanceToImpact / damageRadius));
        float casualtiesInMillions = populationInMillions * coverage;

        Debug.Log($"Region '{regionName}' suffered {casualtiesInMillions:F2} million casualties.");

        return casualtiesInMillions;
    }
}