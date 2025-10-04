using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

// ####################################################################
// ##  UPDATED DATA CLASSES TO STORE ALL NEW FIELDS                  ##
// ####################################################################

public class NasaApiResponse
{
    [JsonProperty("near_earth_objects")]
    public Dictionary<string, List<NearEarthObject>> NearEarthObjects { get; set; }
}

[System.Serializable]
public class NearEarthObject
{
    public string id;
    public string name;
    public EstimatedDiameter estimated_diameter;
    public bool is_potentially_hazardous_asteroid;
    public CloseApproachData[] close_approach_data;
    public OrbitalData orbital_data;

    // --- New Fields Added ---
    [JsonProperty("absolute_magnitude_h")]
    public float absolute_magnitude_h;
    [JsonProperty("is_sentry_object")]
    public bool is_sentry_object;
}

[System.Serializable]
public class EstimatedDiameter
{
    public Kilometers kilometers;
    // Added meters
    public Meters meters;
}

[System.Serializable]
public class Kilometers
{
    public double estimated_diameter_min;
    public double estimated_diameter_max;
}

// --- New Class for Meters ---
[System.Serializable]
public class Meters
{
    public double estimated_diameter_min;
    public double estimated_diameter_max;
}

[System.Serializable]
public class CloseApproachData
{
    public string close_approach_date_full;
    public MissDistance miss_distance;
    // Added relative_velocity
    public RelativeVelocity relative_velocity;
}

// --- New Class for Velocity ---
[System.Serializable]
public class RelativeVelocity
{
    [JsonProperty("kilometers_per_second")]
    public string kilometers_per_second;
    [JsonProperty("kilometers_per_hour")]
    public string kilometers_per_hour;
    [JsonProperty("miles_per_hour")]
    public string miles_per_hour;
}

[System.Serializable]
public class MissDistance { public string kilometers; }

[System.Serializable]
public class OrbitalData
{
    public string semi_major_axis;
    public string eccentricity;
    public string inclination;
    [JsonProperty("orbital_period")]
    public string orbital_period_in_days;
    public string perihelion_distance;
    public string aphelion_distance;
}


// ####################################################################
// ##  YOUR MAIN AsteroidManager CLASS                               ##
// ####################################################################

public class AsteroidManager : MonoBehaviour
{
    // --- Inspector Fields ---
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform earthTransform;
    [SerializeField] private Transform asteroidParent;

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
            var neoList = ParseFeedResponse(webRequest.downloadHandler.text);

            Debug.Log($"Found {neoList.Count} asteroids. Now fetching detailed data for each...");

            foreach (var basicNeo in neoList)
            {
                // Pass the relevant close_approach_data from the initial query
                yield return StartCoroutine(GetDetailedAsteroidData(basicNeo.id, basicNeo.close_approach_data));
            }
        }
    }

    // Modify the GetDetailedAsteroidData coroutine itself
    private IEnumerator GetDetailedAsteroidData(string asteroidId, CloseApproachData[] relevantCloseApproach)
    {
        string lookupUrl = $"{ApiLookupUrl}{asteroidId}?api_key={apiKey}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(lookupUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                NearEarthObject detailedNeo = JsonConvert.DeserializeObject<NearEarthObject>(webRequest.downloadHandler.text);

                // *** IMPORTANT CHANGE ***
                // Overwrite the (potentially irrelevant) close approach data from the lookup
                // with the timely data from our initial feed query.
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
        dataComponent.neoData = neo;

        if (asteroidParent != null)
        {
            asteroidInstance.transform.parent = asteroidParent;
        }

        // --- 1. OBJECT SCALING ---
        // Get the real-world average diameter in kilometers.
        float avgDiameterKm = (float)(neo.estimated_diameter.kilometers.estimated_diameter_min + neo.estimated_diameter.kilometers.estimated_diameter_max) / 2.0f;

        // ** Tweak this value **: A multiplier to control the final visual size.
        float visualScaleMultiplier = 0.5f;

        // Apply a non-linear scale (logarithmic) to make small asteroids visible.
        // The "+1" prevents log(1) from being 0, ensuring a base size.
        float scaledDiameter = Mathf.Log(avgDiameterKm + 1) * visualScaleMultiplier;
        asteroidInstance.transform.localScale = Vector3.one * scaledDiameter;


        // --- 2. DISTANCE SCALING & POSITIONING ---
        // Get the real-world miss distance in kilometers.
        float missKm = float.Parse(neo.close_approach_data[0].miss_distance.kilometers);

        // ** Tweak this value **: A multiplier to bring the vast distances into your scene.
        float distanceScaleMultiplier = 0.1f;

        // Apply a non-linear scale (square root) to compress the vast distances.
        float scaledMissDistance = Mathf.Sqrt(missKm) * distanceScaleMultiplier;

        // Determine a random direction for the asteroid's approach.
        Vector3 randomDirection = Random.onUnitSphere;

        // Set the final position relative to Earth.
        Vector3 position = earthTransform.position + (randomDirection * scaledMissDistance);
        asteroidInstance.transform.position = position;
    }

    // --- UPDATED METHOD THAT AVOIDS 'dynamic' ---
    private List<NearEarthObject> ParseFeedResponse(string jsonResponse)
    {
        // Deserialize the entire response into our new helper class
        var apiResponse = JsonConvert.DeserializeObject<NasaApiResponse>(jsonResponse);

        var allNeos = new List<NearEarthObject>();

        // The data is now in a clean dictionary we can loop through
        foreach (var dateEntry in apiResponse.NearEarthObjects.Values)
        {
            allNeos.AddRange(dateEntry);
        }

        return allNeos;
    }
}