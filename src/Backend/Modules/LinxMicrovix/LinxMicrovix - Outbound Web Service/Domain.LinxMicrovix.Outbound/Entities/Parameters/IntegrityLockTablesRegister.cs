namespace Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters
{
    public class IntegrityLockTablesRegister
    {
        public string? table { get; set; }
        public string? recordKey { get; set; }
        public string? identifier { get; set; }
        public bool is_present_in_erp { get; set; }
    }
}
