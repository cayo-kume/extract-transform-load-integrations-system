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

        /// <summary>
        /// próximo horário, estritamente depois de <paramref name="after"/>, pelas mesmas regras do cron acima, no fuso da agenda:
        /// diário na hora de RunningDailyAt; N minutos que dividem a hora, contados a partir da hora cheia; N horas que dividem o
        /// dia, no minuto 0. O WebJob usa isto no lugar de um parser de cron. Null quando a agenda não vira cron
        /// </summary>
        public static DateTimeOffset? NextOccurrence(int intervalTimeInMinutes, TimeSpan? runningDailyAt, DateTimeOffset after, TimeZoneInfo timeZone)
        {
            if (From(intervalTimeInMinutes, runningDailyAt) is null)
                return null;

            // horário local do fuso, sem os segundos quebrados: o próximo horário é sempre um minuto cheio depois dele
            var local = TimeZoneInfo.ConvertTime(after, timeZone).DateTime;
            var minute = new DateTime(local.Year, local.Month, local.Day, local.Hour, local.Minute, 0, DateTimeKind.Unspecified);

            DateTime next;

            if (runningDailyAt is TimeSpan at)
            {
                next = local.Date + at;

                if (next <= local)
                    next = next.AddDays(1);
            }
            else if (intervalTimeInMinutes < 60)
            {
                var hour = minute.AddMinutes(-minute.Minute);
                next = hour.AddMinutes((minute.Minute / intervalTimeInMinutes + 1) * intervalTimeInMinutes);
            }
            else
            {
                var hours = intervalTimeInMinutes / 60;
                next = local.Date.AddHours((local.Hour / hours + 1) * hours);
            }

            // um horário que não existe no fuso (início de horário de verão) vai para a primeira hora válida depois dele
            while (timeZone.IsInvalidTime(next))
                next = next.AddMinutes(1);

            return new DateTimeOffset(next, timeZone.GetUtcOffset(next));
        }
    }
}
