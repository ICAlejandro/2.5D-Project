using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDurationInSeconds = 60f; 

    [Header("Current Time Data")]
    [Range(0f, 1f)]
    public float currentTimeOfDay = 0f; 

    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
    }

    void Update()
    {
        if (timeManager != null)
        {
            // Sync internal state directly with TimeManager's master cycle
            currentTimeOfDay = timeManager.currentTimeOfDay;
            dayDurationInSeconds = timeManager.dayDurationInSeconds;
        }

        // Apply smooth celestial light rotation
        float sunXRotation = currentTimeOfDay * 360f;
        transform.rotation = Quaternion.Euler(sunXRotation - 90f, 170f, 0f);
    }
}