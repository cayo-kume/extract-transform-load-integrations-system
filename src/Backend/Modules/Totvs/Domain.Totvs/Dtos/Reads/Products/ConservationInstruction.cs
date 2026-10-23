namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// instrução de conservação do produto (ProductInstructionModel)
    /// </summary>
    public class ConservationInstruction
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? @default { get; init; }
        public string? grouperCode { get; init; }

        public List<ConservationInstructionItem>? items { get; init; }
    }
}
