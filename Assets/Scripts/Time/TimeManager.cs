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

    private const int HoursInDay = 24;
    private const int MinutesInHour = 60;
    private const int SecondsInMinute = 60;
    private const int DaysInMonth = 30; 
    private const int MonthsInYear = 12;

    void Update()
    {
        CalculateInGameClock();
    }

    private void CalculateInGameClock()
    {
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay -= 1f;
            AdvanceCalendarDay();
        }

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

    public string GetFormattedDate()
    {
        return $"{currentMonth:D2}/{currentDay:D2}/{currentYear}";
    }

    public string GetFormattedTime()
    {
        return $"{currentHour:D2}:{currentMinute:D2}";
    }
}