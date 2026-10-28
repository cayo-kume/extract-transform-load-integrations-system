namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// código de barras incluído (BarCodeInsertModel): quantity é inteiro no schema. inRfid grava o RFID como "Em andamento"
    /// </summary>
    public class BarCodeInsert
    {
        public string? barCode { get; init; }
        public int? quantity { get; init; }
        public bool? isMainCode { get; init; }
        public bool? inRfid { get; init; }
    }
}
