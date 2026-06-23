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
        timeManager = FindFirstObjectByType<TimeManager>();

        if (timeManager == null)
        {
            Debug.LogError("Rest.cs could not find a TimeManager in the scene!");
        }

        // Set the default dialogue text shown when the player walks up
        dialogueText = "Press Enter to rest and skip to the next day.";
    }

    void Update()
    {
        // If we're fast-forwarding, check if the day has rolled over yet
        if (isFastForwarding && timeManager != null)
        {
            // TimeManager's AdvanceCalendarDay is called internally when currentTimeOfDay resets to 0.
            // We detect the rollover by checking if the time wrapped back near 0.
            if (timeManager.currentTimeOfDay < 0.01f)
            {
                StopFastForward();
            }
        }
    }

    public override void Interact(GameObject playerObject)
    {
        if (timeManager == null)
        {
            Debug.LogWarning("Rest: No TimeManager found, cannot skip day.");
            return;
        }

        if (isFastForwarding) return; // Don't stack interactions

        if (instantSkip)
        {
            SkipToNextDay();
        }
        else
        {
            StartFastForward();
        }
    }

    // Instantly jumps to the start of the next day
    private void SkipToNextDay()
    {
        // Setting currentTimeOfDay to 1f will cause TimeManager.CalculateInGameClock()
        // to fire AdvanceCalendarDay() on its very next Update tick, then reset to 0.
        // This means no calendar logic is duplicated here — TimeManager handles it cleanly.
        timeManager.currentTimeOfDay = 1f;

        Debug.Log("Rested! Skipping to the next day.");
    }

    // Speeds up time so the day rolls over faster (visible fast-forward effect)
    private void StartFastForward()
    {
        isFastForwarding = true;
        originalDayDuration = timeManager.dayDurationInSeconds;

        // Shrink the remaining time into the fastForwardDuration window
        float remainingFraction = 1f - timeManager.currentTimeOfDay;
        if (remainingFraction <= 0f) remainingFraction = 1f;

        // Temporarily set dayDuration so the remaining time passes in fastForwardDuration seconds
        timeManager.dayDurationInSeconds = originalDayDuration * remainingFraction / fastForwardDuration;

        Debug.Log($"Rest: Fast-forwarding to next day over {fastForwardDuration} real second(s).");
    }

    // Called when the fast-forward completes (day rolled over)
    private void StopFastForward()
    {
        isFastForwarding = false;
        timeManager.dayDurationInSeconds = originalDayDuration; // Restore normal speed
        Debug.Log("Rest: Fast-forward complete. Day has changed, time restored to normal speed.");
    }
}
