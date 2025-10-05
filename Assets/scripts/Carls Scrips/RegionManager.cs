using UnityEngine;
using System.Collections.Generic;

public class RegionManager : MonoBehaviour
{
    public static RegionManager Instance;

    public List<PopulationRegion> allRegions = new List<PopulationRegion>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}