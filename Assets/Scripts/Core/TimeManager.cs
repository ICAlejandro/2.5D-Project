using UnityEngine;

public class TimeManager : MonoBehaviour, ITimeProvider
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
    public int currentDay   = 1;
    public int currentMonth = 1;
    public int currentYear  = 2026;

    private const int HoursInDay      = 24;
    private const int MinutesInHour   = 60;
    private const int SecondsInMinute = 60;
    private const int DaysInMonth     = 30;
    private const int MonthsInYear    = 12;

    // ── ITimeProvider ──────────────────────────────────────────────────────
    public float CurrentTimeOfDay
    {
        get => currentTimeOfDay;
        set => currentTimeOfDay = value;
    }

    public float DayDurationInSeconds
    {
        get => dayDurationInSeconds;
        set => dayDurationInSeconds = value;
    }

    public int CurrentDay   => currentDay;
    public int CurrentMonth => currentMonth;
    public int CurrentYear  => currentYear;

    // ── Unity Lifecycle ────────────────────────────────────────────────────
    void Awake()
    {
        ServiceLocator.Register<ITimeProvider>(this);
        Debug.Log("[TimeManager] Registered as ITimeProvider.");
    }

    void Update()
    {
        CalculateInGameClock();
    }

    // ── ITimeProvider methods ──────────────────────────────────────────────
    public string GetFormattedDate()
        => $"{currentMonth:D2}/{currentDay:D2}/{currentYear}";

    public string GetFormattedTime()
    {
        string period = currentHour >= 12 ? "PM" : "AM";
        int hour12    = currentHour % 12;
        if (hour12 == 0) hour12 = 12;
        return $"{hour12}:{currentMinute:D2} {period}";
    }

    // ── Private ────────────────────────────────────────────────────────────
    private void CalculateInGameClock()
    {
        if (dayDurationInSeconds <= 0) dayDurationInSeconds = 1f;

        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
            AdvanceCalendarDay();
        }

        float totalSecondsInDay     = HoursInDay * MinutesInHour * SecondsInMinute;
        float currentSecondsElapsed = currentTimeOfDay * totalSecondsInDay;

        currentHour   = Mathf.FloorToInt(currentSecondsElapsed / (MinutesInHour * SecondsInMinute));
        float remMins = currentSecondsElapsed % (MinutesInHour * SecondsInMinute);
        currentMinute = Mathf.FloorToInt(remMins / SecondsInMinute);
        currentSecond = Mathf.FloorToInt(remMins % SecondsInMinute);
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
}
