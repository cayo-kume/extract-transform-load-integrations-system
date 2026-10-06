namespace Domain.Shared.Entities.Base
{
    public abstract class CompanyBase
    {
        public string? cod_company { get; init; }

        public string? doc_company { get; init; }

        public string? reason_company { get; init; }

        public string? name_company { get; init; }

        public string? email_company { get; init; }

        public string? address_company { get; init; }

        public string? street_number_company { get; init; }

        public string? complement_address_company { get; init; }

        public string? neighborhood_company { get; init; }

        public string? city_company { get; init; }

        public string? uf_company { get; init; }

        public string? zip_code_company { get; init; }

        public string? fone_company { get; init; }

        public string? state_registration_company { get; init; }

        public string? municipal_registration_company { get; init; }
    }
}
