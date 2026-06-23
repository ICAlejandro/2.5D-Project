using UnityEngine;

public class Rest : Interactable
{
    [Header("Rest Settings")]
    [Tooltip("If true, pressing Enter while near this object skips to the next day. If false, it fast-forwards in real time.")]
    public bool instantSkip = true;

    [Tooltip("Only used if Instant Skip is OFF — how many real seconds the fast-forward takes.")]
    public float fastForwardDuration = 2f;

    private TimeManager timeManager;
    private bool isFastForwarding = false;
    private float originalDayDuration;

    void Start()
    {
        // Resolve from ServiceLocator instead of searching the scene
        timeManager = ServiceLocator.Get<TimeManager>();

        if (timeManager == null)
            Debug.LogError("Rest.cs could not find a TimeManager via ServiceLocator!");

        dialogueText = "Press Enter to rest and skip to the next day.";
    }

    void Update()
    {
        if (isFastForwarding && timeManager != null)
        {
            if (timeManager.currentTimeOfDay < 0.01f)
                StopFastForward();
        }
    }

    public override void Interact(GameObject playerObject)
    {
        if (timeManager == null)
        {
            Debug.LogWarning("Rest: No TimeManager found, cannot skip day.");
            return;
        }

        if (isFastForwarding) return;

        if (instantSkip) SkipToNextDay();
        else             StartFastForward();
    }

    private void SkipToNextDay()
    {
        timeManager.currentTimeOfDay = 1f;
        Debug.Log("Rested! Skipping to the next day.");
    }

    private void StartFastForward()
    {
        isFastForwarding = true;
        originalDayDuration = timeManager.dayDurationInSeconds;

        float remainingFraction = 1f - timeManager.currentTimeOfDay;
        if (remainingFraction <= 0f) remainingFraction = 1f;

        timeManager.dayDurationInSeconds = originalDayDuration * remainingFraction / fastForwardDuration;
        Debug.Log($"Rest: Fast-forwarding to next day over {fastForwardDuration} real second(s).");
    }

    private void StopFastForward()
    {
        isFastForwarding = false;
        timeManager.dayDurationInSeconds = originalDayDuration;
        Debug.Log("Rest: Fast-forward complete. Day has changed, time restored to normal speed.");
    }
}
