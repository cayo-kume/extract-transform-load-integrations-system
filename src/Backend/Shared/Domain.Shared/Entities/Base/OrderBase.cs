namespace Domain.Shared.Entities.Base
{
    public abstract class OrderBase
    {
        public string? number { get; init; }

        public string? cfop { get; init; }

        public int volumes { get; init; }
    }
}
