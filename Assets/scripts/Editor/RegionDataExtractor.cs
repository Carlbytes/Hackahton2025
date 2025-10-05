using UnityEngine;
using UnityEditor; // Required for creating editor tools
using System.Text; // Required for building the report string

public class RegionDataExtractor
{
    // This creates a new menu item in the Unity editor at the top: "Tools/Log All Region Data"
    [MenuItem("Tools/Log All Region Data")]
    private static void LogAllRegionData()
    {
        // Find every active PopulationRegion component in the current scene.
        PopulationRegion[] allRegions = Object.FindObjectsOfType<PopulationRegion>();

        if (allRegions.Length == 0)
        {
            Debug.Log("No PopulationRegion components found in the scene.");
            return;
        }

        // Use a StringBuilder to create a neat report.
        StringBuilder report = new StringBuilder();
        report.AppendLine($"--- POPULATION REGION REPORT ({allRegions.Length} regions found) ---");

        float totalPopulation = 0;

        // Loop through each region we found.
        foreach (PopulationRegion region in allRegions)
        {
            // Add its name and population to our report string.
            report.AppendLine($"Region: {region.regionName}, Population: {region.populationInMillions} Million");
            totalPopulation += region.populationInMillions;
        }

        report.AppendLine("--------------------");
        report.AppendLine($"TOTAL WORLD POPULATION: {totalPopulation:F2} Million");

        // Print the final, complete report to the console.
        Debug.Log(report.ToString());
    }
}