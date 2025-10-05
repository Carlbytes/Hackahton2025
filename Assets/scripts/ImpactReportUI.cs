using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ImpactReportUI : MonoBehaviour
{
    public static ImpactReportUI Instance;

    [SerializeField]
    private TextMeshProUGUI reportText;

    private CanvasGroup canvasGroup;

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

        // Ensure CanvasGroup exists.
        canvasGroup = GetComponent<CanvasGroup>();

        // If CanvasGroup is missing for some reason, add it dynamically.
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.LogWarning("CanvasGroup was missing and has been added at runtime.");
        }

        // Try to get the TextMeshProUGUI component if it's not assigned.
        if (reportText == null)
        {
            reportText = GetComponentInChildren<TextMeshProUGUI>();
            if (reportText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found! Please assign it in the Inspector.");
            }
        }

        // Start with the panel completely invisible and non-interactive.
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Displays the report by fading in the CanvasGroup.
    /// </summary>
    public void DisplayReport(string reportContent)
    {
        // Check if the canvasGroup is still null at this point (defensive programming)
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is still null when trying to display report!");
            return;
        }

        if (reportText == null)
        {
            Debug.LogWarning("reportText is not assigned in the Inspector!");
            return;
        }

        // Set the text content first.
        reportText.text = reportContent;

        // Display the panel.
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
