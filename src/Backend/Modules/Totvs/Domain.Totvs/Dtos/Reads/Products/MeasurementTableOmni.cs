namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// tabela de medidas omni da cor (MeasurementTableModel); exige option.measurementTableOmniType
    /// </summary>
    public class MeasurementTableOmni
    {
        public string? sku { get; init; }
        public string? tamanho { get; init; }

        public List<Medida>? medidas { get; init; }
    }
}
