namespace Domain.Totvs.Dtos.Product
{
    public class ConservationInstruction
    {
        public string code { get; init; }
        public string description { get; init; }
        public string _default { get; init; }
        public string grouperCode { get; init; }
        public List<ConservationInstructionsItem>? items { get; init; }
    }
}
