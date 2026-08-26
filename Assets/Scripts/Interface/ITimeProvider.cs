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
