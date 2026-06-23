using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Calibration")]
    [Tooltip("How long one full in-game day takes in real-world seconds.")]
    public float dayDurationInSeconds = 60f;

    [Header("Current Time Data Source")]
    [Range(0f, 1f)]
    public float currentTimeOfDay = 0f;

    [Header("Calculated Values (Read Only)")]
    public int currentSecond;
    public int currentMinute;
    public int currentHour;
    public int currentDay = 1;
    public int currentMonth = 1;
    public int currentYear = 2026;

    // Fixed structure configuration
    private const int HoursInDay = 24;
    private const int MinutesInHour = 60;
    private const int SecondsInMinute = 60;
    private const int DaysInMonth = 30; // Unified 30-day cycle for simulation simplicity
    private const int MonthsInYear = 12;

    void Update()
    {
        CalculateInGameClock();
    }

    private void CalculateInGameClock()
    {
        // 1. Advance the raw 0-to-1 fractional progress tracker
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;

        // 2. If a complete day rotation passes, increment calendar numbers
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay -= 1f;
            AdvanceCalendarDay();
        }

        // 3. Translate the fractional value into human-readable digital clock metrics
        float totalSecondsInDay = HoursInDay * MinutesInHour * SecondsInMinute;
        float currentSecondsElapsed = currentTimeOfDay * totalSecondsInDay;

        currentHour = Mathf.FloorToInt(currentSecondsElapsed / (MinutesInHour * SecondsInMinute));
        float remainderMinutes = currentSecondsElapsed % (MinutesInHour * SecondsInMinute);

        currentMinute = Mathf.FloorToInt(remainderMinutes / SecondsInMinute);
        currentSecond = Mathf.FloorToInt(remainderMinutes % SecondsInMinute);
    }

    private void AdvanceCalendarDay()
    {
        currentDay++;
        if (currentDay > DaysInMonth)
        {
            currentDay = 1;
            currentMonth++;

            if (currentMonth > MonthsInYear)
            {
                currentMonth = 1;
                currentYear++;
            }
        }
    }

    /// <summary>
    /// Returns a formatted string representing the date: MM/DD/YYYY
    /// </summary>
    public string GetFormattedDate()
    {
        return $"{currentMonth:D2}/{currentDay:D2}/{currentYear}";
    }

    /// <summary>
    /// Returns a formatted string representing the time: HH:MM
    /// </summary>
    public string GetFormattedTime()
    {
        return $"{currentHour:D2}:{currentMinute:D2}";
    }
}