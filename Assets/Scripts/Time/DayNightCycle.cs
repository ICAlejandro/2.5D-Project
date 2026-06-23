using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Manager Link")]
    [Tooltip("The master TimeManager script tracking the game calendar.")]
    private TimeManager timeManager;

    [Header("Current Sync Data (Read Only)")]
    [Range(0f, 1f)]
    public float currentTimeOfDay = 0f; 

    void Start()
    {
        // Automatically find the master clock in the scene
        timeManager = FindFirstObjectByType<TimeManager>();
        
        if (timeManager == null)
        {
            Debug.LogError("DayNightCycle could not find a TimeManager in the scene! Please make sure one exists.");
        }
    }

    void Update()
    {
        if (timeManager != null)
        {
            // Directly pull the current day's progress from the master TimeManager
            currentTimeOfDay = timeManager.currentTimeOfDay;
        }

        // Apply smooth celestial light rotation based on the TimeManager's 0 to 1 value
        // 0.0 = Midnight, 0.25 = Sunrise, 0.5 = Noon, 0.75 = Sunset
        float sunXRotation = currentTimeOfDay * 360f;
        
        // Offset by -90 degrees so that 0.5 (Noon) points straight down (peak brightness)
        transform.rotation = Quaternion.Euler(sunXRotation - 90f, 170f, 0f);
    }
}