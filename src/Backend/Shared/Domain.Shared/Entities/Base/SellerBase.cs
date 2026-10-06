namespace Domain.Shared.Entities.Base
{
    public abstract class SellerBase
    {
        public string? cod_client { get; init; }

        public string? doc_client { get; init; }

        public string? reason_client { get; init; }

        public string? name_client { get; init; }

        public string? email_client { get; init; }

        public string? address_client { get; init; }

        public string? street_number_client { get; init; }

        public string? complement_address_client { get; init; }

        public string? neighborhood_client { get; init; }

        public string? city_client { get; init; }

        public string? uf_client { get; init; }

        public string? zip_code_client { get; init; }

        public string? fone_client { get; init; }

        public string? state_registration_client { get; init; }

        public string? municipal_registration_client { get; init; }
    }
}
