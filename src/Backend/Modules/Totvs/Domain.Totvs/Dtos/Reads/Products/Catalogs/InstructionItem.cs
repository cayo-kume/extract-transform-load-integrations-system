namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// item de instrução (GET product/v2/instruction-items, InstructionItemResultModel; sem as imagens, PR13)
    /// </summary>
    public class InstructionItem
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? grouper { get; init; }
        public string? grouperDescription { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
