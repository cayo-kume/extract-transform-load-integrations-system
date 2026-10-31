namespace Hosts.Scheduling
{
    /// <summary>
    /// conversão da agenda de auditing.Jobs em cron (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2). O cron conta a partir da
    /// hora cheia, então só entram intervalos que ele representa exatamente; o resto fica fora da agenda, com aviso no log
    /// </summary>
    public static class JobCron
    {
        /// <summary>
        /// cron do job, ou null quando a agenda não vira cron (intervalo 0, que não divide a hora ou o dia, ou diário sem hora)
        /// </summary>
        public static string? From(int intervalTimeInMinutes, TimeSpan? runningDailyAt)
        {
            // RunningDailyAt preenchida: uma vez por dia, nessa hora (§4.1)
            if (runningDailyAt is TimeSpan at)
                return at >= TimeSpan.Zero && at < TimeSpan.FromDays(1) ? $"{at.Minutes} {at.Hours} * * *" : null;

            return FromInterval(intervalTimeInMinutes);
        }

        public static string? FromInterval(int intervalTimeInMinutes) =>
            intervalTimeInMinutes switch
            {
                1 => "* * * * *",
                > 1 and < 60 when 60 % intervalTimeInMinutes == 0 => $"*/{intervalTimeInMinutes} * * * *",
                60 => "0 * * * *",
                > 60 and < 1440 when intervalTimeInMinutes % 60 == 0 && 24 % (intervalTimeInMinutes / 60) == 0 => $"0 */{intervalTimeInMinutes / 60} * * *",
                _ => null
            };
    }
}
