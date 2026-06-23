using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Current Sync Data (Read Only)")]
    [Range(0f, 1f)]
    public float currentTimeOfDay = 0f;

    private TimeManager timeManager;

    void Start()
    {
        // Resolve from ServiceLocator instead of searching the scene
        timeManager = ServiceLocator.Get<TimeManager>();

        if (timeManager == null)
            Debug.LogError("DayNightCycle could not find a TimeManager via ServiceLocator!");
    }

    void Update()
    {
        if (timeManager != null)
            currentTimeOfDay = timeManager.currentTimeOfDay;

        // 0.0 = Midnight, 0.25 = Sunrise, 0.5 = Noon, 0.75 = Sunset
        float sunXRotation = currentTimeOfDay * 360f;
        transform.rotation = Quaternion.Euler(sunXRotation - 90f, 170f, 0f);
    }
}
