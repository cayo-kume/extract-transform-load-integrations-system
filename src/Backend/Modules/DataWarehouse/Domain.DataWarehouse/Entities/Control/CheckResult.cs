namespace Domain.DataWarehouse.Entities.Control
{
    /// <summary>
    /// resultado de uma checagem de qualidade, tabela dw_control.CheckResult (src/Backend/Modules/DataWarehouse/docs, 9.2). Fica fora da
    /// limpeza de 15 dias do auditing (3.9)
    /// </summary>
    public class CheckResult
    {
        public string check_name { get; private set; } = String.Empty;
        public DateTime executed_at { get; private set; }
        public int error_rows { get; private set; }
        public bool passed { get; private set; }

        public CheckResult() { }

        public CheckResult(string check_name, int error_rows)
        {
            this.check_name = check_name?.Length > 100 ? check_name[..100] : check_name ?? String.Empty;
            executed_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.error_rows = error_rows;
            passed = error_rows == 0;
        }
    }
}
