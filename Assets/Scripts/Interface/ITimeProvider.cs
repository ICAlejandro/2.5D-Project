/// <summary>
/// Contract for any time system. Scripts depend on this instead of TimeManager directly.
/// Swap out the implementation freely without touching any consumer script.
/// </summary>
public interface ITimeProvider
{
    float CurrentTimeOfDay     { get; set; }
    float DayDurationInSeconds { get; set; }
    int   CurrentDay           { get; }
    int   CurrentMonth         { get; }
    int   CurrentYear          { get; }

    string GetFormattedDate();
    string GetFormattedTime();
}
