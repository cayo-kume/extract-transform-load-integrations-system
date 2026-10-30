namespace Domain.DataWarehouse.Entities.Control
{
    /// <summary>
    /// origem do DW, tabela dw_control.LandingSource (src/Backend/Modules/DataWarehouse/docs, 9.5): o job chama as procedures geradas para cada uma
    /// </summary>
    public class LandingSource
    {
        public string source_schema { get; set; } = String.Empty;
        public string source_table { get; set; } = String.Empty;
        public bool detect_missing { get; set; }

        // hourly, daily ou nulo (sem histórico)
        public string? history_frequency { get; set; }
    }
}
