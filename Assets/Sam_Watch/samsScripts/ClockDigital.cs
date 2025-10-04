using TMPro;
using UnityEngine;

// Make sure to include this for TextMeshPro

namespace samsScripts
{
    public class ClockDigital : MonoBehaviour
    {
        [Header("Timer Settings")]
        [Tooltip("The time to start the countdown from, in seconds.")]
        public float timeRemaining = 120; // 2 minutes for example
        public bool timerIsRunning = false;

        [Header("UI References")]
        [Tooltip("The TextMeshPro UI element to display the time.")]
        public TMP_Text timerText;

        void Start()
        {
            // To start the timer as soon as the game begins
            timerIsRunning = true;
        }

        void Update()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    // Subtract the time that has passed since the last frame
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    // Timer has reached zero
                    Debug.Log("Time has run out!");
                    timeRemaining = 0;
                    timerIsRunning = false;
                    DisplayTime(timeRemaining);
                    // You could add an event here, like ending the game or triggering something
                }
            }
        }

        // This method formats the float time into a minutes:seconds format
        void DisplayTime(float timeToDisplay)
        {
            // We add 1 second because the display would otherwise floor down,
            // showing 00:00 when there's still a fraction of a second left.
            timeToDisplay += 1;

            float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            // Formats the string to always show two digits, e.g., 01:05
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Public method to start the timer from another script if needed
        public void StartTimer()
        {
            timerIsRunning = true;
        }
    }
}