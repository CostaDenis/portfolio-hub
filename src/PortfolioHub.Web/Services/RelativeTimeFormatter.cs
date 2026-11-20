namespace PortfolioHub.Web.Services;

public static class RelativeTimeFormatter
{
    // "agora mesmo" · "há 5 min" · "há 2 h" · "há 3 dias"
    public static string Format(DateTime utc, DateTime nowUtc)
    {
        var elapsed = nowUtc - utc;

        if (elapsed < TimeSpan.FromMinutes(1))
            return "agora mesmo";
        if (elapsed < TimeSpan.FromHours(1))
            return $"há {(int)elapsed.TotalMinutes} min";
        if (elapsed < TimeSpan.FromDays(1))
            return $"há {(int)elapsed.TotalHours} h";

        var days = (int)elapsed.TotalDays;
        return days == 1 ? "há 1 dia" : $"há {days} dias";
    }

    // O banco devolve a data sem fuso (Kind = Unspecified), mas ela é gravada em UTC.
    public static DateTime AsUtc(DateTime value)
        => value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
}
