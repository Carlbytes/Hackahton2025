using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;


public class AsteroidManager : MonoBehaviour
{
    // --- Inspector Fields ---
    [SerializeField] private string apiKey = "Bp0CgXhTRN0qpt4qpgaAggzMdBBEc2lVJWgbkDTv"; 
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform earthTransform;
    [SerializeField] private Transform asteroidParent;

    [Header("UI Board Settings")]
    [Tooltip("The parent object where UI buttons will be created.")]
    [SerializeField] private Transform uiButtonParent;
    [Tooltip("The button prefab for the UI board.")]
    [SerializeField] private GameObject asteroidButtonPrefab;

    // --- Private Fields ---
    private const string ApiFeedUrl = "https://api.nasa.gov/neo/rest/v1/feed";
    private const string ApiLookupUrl = "https://api.nasa.gov/neo/rest/v1/neo/";

    void Start()
    {
        StartCoroutine(FetchAsteroidListAndDetails());
    }

    private IEnumerator FetchAsteroidListAndDetails()
    {
        string startDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        string endDate = System.DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
        string feedUrl = $"{ApiFeedUrl}?start_date={startDate}&end_date={endDate}&api_key={apiKey}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(feedUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching asteroid list: {webRequest.error}");
                yield break;
            }

            Debug.Log("Successfully received asteroid list!");
            var initialNeoList = ParseFeedResponse(webRequest.downloadHandler.text);

            var validNeoList = initialNeoList
                .Where(neo => neo.close_approach_data != null && neo.close_approach_data.Length > 0)
                .ToList();

            Debug.Log($"Found {initialNeoList.Count} total asteroids, {validNeoList.Count} have valid data. Fetching details...");

            foreach (var validNeo in validNeoList)
            {
                yield return StartCoroutine(GetDetailedAsteroidData(validNeo.id, validNeo.close_approach_data));
            }
        }
    }

    private IEnumerator GetDetailedAsteroidData(string asteroidId, CloseApproachData[] relevantCloseApproach)
    {
        string lookupUrl = $"{ApiLookupUrl}{asteroidId}?api_key={apiKey}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(lookupUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                NearEarthObject detailedNeo = JsonConvert.DeserializeObject<NearEarthObject>(webRequest.downloadHandler.text);
                detailedNeo.close_approach_data = relevantCloseApproach;
                CreateVRAsteroid(detailedNeo);
            }
            else
            {
                Debug.LogError($"Error fetching details for asteroid {asteroidId}: {webRequest.error}");
            }
        }
    }

    private void CreateVRAsteroid(NearEarthObject neo)
    {
        if (asteroidPrefab == null || earthTransform == null) return;

        GameObject asteroidInstance = Instantiate(asteroidPrefab, earthTransform.position, Quaternion.identity);
        asteroidInstance.name = neo.name;

        AsteroidData dataComponent = asteroidInstance.AddComponent<AsteroidData>();
        asteroidInstance.AddComponent<AsteroidImpact>(); 
        dataComponent.neoData = neo;

        if (asteroidParent != null)
        {
            asteroidInstance.transform.parent = asteroidParent;
        }

        float avgDiameter = (float)(neo.estimated_diameter.kilometers.estimated_diameter_min + neo.estimated_diameter.kilometers.estimated_diameter_max) / 2.0f;
        float visualScaleMultiplier = 0.1f;
        asteroidInstance.transform.localScale = Vector3.one * avgDiameter * visualScaleMultiplier;

        float missKm = float.Parse(neo.close_approach_data[0].miss_distance.kilometers);
        float sceneDistanceMultiplier = 0.00001f;
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 position = earthTransform.position + (randomDirection * missKm * sceneDistanceMultiplier);
        asteroidInstance.transform.position = position;

        if (uiButtonParent != null && asteroidButtonPrefab != null)
        {
            GameObject buttonInstance = Instantiate(asteroidButtonPrefab, uiButtonParent);
            AsteroidButtonController buttonController = buttonInstance.GetComponent<AsteroidButtonController>();
            if (buttonController != null)
            {
                buttonController.Initialize(neo, asteroidInstance.transform);
            }
        }
    }

    private List<NearEarthObject> ParseFeedResponse(string jsonResponse)
    {
        var apiResponse = JsonConvert.DeserializeObject<NasaApiResponse>(jsonResponse);
        var allNeos = new List<NearEarthObject>();
        foreach (var dateEntry in apiResponse.NearEarthObjects.Values)
        {
            allNeos.AddRange(dateEntry);
        }
        return allNeos;
    }
}