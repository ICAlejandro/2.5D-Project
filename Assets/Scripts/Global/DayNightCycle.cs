using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Current Sync Data (Read Only)")]
    [Range(0f, 1f)]
    public float currentTimeOfDay = 0f;

    private ITimeProvider timeProvider;

    void Start()
    {
        timeProvider = ServiceLocator.Get<ITimeProvider>();

        if (timeProvider == null)
            Debug.LogError("DayNightCycle could not find an ITimeProvider via ServiceLocator!");
    }

    void Update()
    {
        if (timeProvider != null)
            currentTimeOfDay = timeProvider.CurrentTimeOfDay;

        // 0.0 = Midnight, 0.25 = Sunrise, 0.5 = Noon, 0.75 = Sunset
        float sunXRotation = currentTimeOfDay * 360f;
        transform.rotation = Quaternion.Euler(sunXRotation - 90f, 170f, 0f);
    }
}
