namespace backend.Helpers;

public static class DateHelper
{
    public static DateTime Now()
    {
        return DateTime.UtcNow;
    }

    public static string Format(DateTime date)
    {
        return date.ToLocalTime().ToString("dd/MM/yyyy");
    }

    public static string FormatWithTime(DateTime date)
    {
        return date.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }
}