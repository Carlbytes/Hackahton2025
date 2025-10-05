using System.Collections.Generic;
using Newtonsoft.Json;


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
    [JsonProperty("absolute_magnitude_h")]
    public float absolute_magnitude_h;
    [JsonProperty("is_sentry_object")]
    public bool is_sentry_object;
}

[System.Serializable]
public class EstimatedDiameter
{
    public Kilometers kilometers;
    public Meters meters;
}

[System.Serializable]
public class Kilometers
{
    public double estimated_diameter_min;
    public double estimated_diameter_max;
}

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
    public RelativeVelocity relative_velocity;
}

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