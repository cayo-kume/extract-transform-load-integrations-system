namespace Domain.DataWarehouse.Entities.Control
{
    /// <summary>
    /// ponto de parada de uma carga, tabela dw_control.Watermark (docs/datawarehouse, 8.1). Uma linha por fonte e entidade:
    /// as origens lidas pela landing, os snapshots (source = dw_history) e a varredura de cada módulo de saída (4.2)
    /// </summary>
    public class Watermark
    {
        public string source { get; set; } = String.Empty;
        public string entity { get; set; } = String.Empty;

        // horário de Brasília: integrated_or_updated_at da origem, loaded_at da landing ou updated_at da view de saída
        public DateTime last_value { get; set; }
        public DateTime updated_at { get; set; }
    }
}
