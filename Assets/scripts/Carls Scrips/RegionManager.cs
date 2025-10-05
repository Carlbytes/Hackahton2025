using UnityEngine;
using System.Collections.Generic;

public class RegionManager : MonoBehaviour
{
    // Static instance for easy access from any script
    public static RegionManager Instance;

    // A list to hold all the population regions in your scene
    public List<PopulationRegion> allRegions = new List<PopulationRegion>();

    void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Automatically find and add all PopulationRegion components in the scene to the list
        allRegions.AddRange(FindObjectsOfType<PopulationRegion>());
    }
}