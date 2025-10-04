using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AssignPopulationValues : MonoBehaviour
{

    [SerializeField] private List<GameObject> regions;
    //[SerializeField] private List<int> populations;
    public RegionTemplate pop;
    //[SerializeField] private RegionTemplate na, usa, ca, saNorth, saSouth, euNorth, euWest, euEast, ruWest,
    //ruEast, saudiPlus, stans, indiaPlus, asiaEast, asiaSouth, oceana, africaNorth, africaEast, africaSouth,
    //antartica;

    void Start()
    {
        
    }
}
