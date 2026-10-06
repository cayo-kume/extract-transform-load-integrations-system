namespace Domain.Shared.Entities.Base
{
    public abstract class InvoiceBase
    {
        public string? number_nf { get; init; }

        public string? key_nfe_nf { get; init; }

        public string? xml_nf { get; init; }

        public string? xml_distribuition_nf { get; init; }

        public decimal amount_nf { get; init; }

        public decimal shipping_value_nf { get; init; }

        public decimal weight_nf { get; init; }

        public string? type_nf { get; } = "NF";

        public string? serie_nf { get; init; }

        public DateTime date_emission_nf { get; init; }
    }
}
