using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.SalesRepresentatives
{
    /// <summary>
    /// endereço do vendedor (a API não tem id do endereço: a chave é a posição na lista), tabela linxcommerce.LinxCommerceSalesRepresentativesAddresses
    /// </summary>
    public class SalesRepresentativeAddress
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int SalesRepresentativeID { get; private set; }
        public int AddressIndex { get; private set; }
        public bool? IsMainAddress { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string ContactName { get; private set; } = String.Empty;
        public string AddressLine { get; private set; } = String.Empty;
        public string Number { get; private set; } = String.Empty;
        public string Neighbourhood { get; private set; } = String.Empty;
        public string City { get; private set; } = String.Empty;
        public string State { get; private set; } = String.Empty;
        public string PostalCode { get; private set; } = String.Empty;
        public string AddressNotes { get; private set; } = String.Empty;
        public string Landmark { get; private set; } = String.Empty;
        public decimal? Latitude { get; private set; }
        public decimal? Longitude { get; private set; }

        public SalesRepresentativeAddress() { }

        public SalesRepresentativeAddress(string doc_company, DateTime integrated_or_updated_at, int SalesRepresentativeID, int AddressIndex, Dtos.Reads.SalesRepresentatives.SalesRepresentativeAddress dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.SalesRepresentativeID = SalesRepresentativeID;
            this.AddressIndex = AddressIndex;

            IsMainAddress = NullableConverters.ToNullableBoolean(dto.IsMainAddress);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            ContactName = ConverterValidatorsExtensions.StringLengthValidation(dto.ContactName, 250);
            AddressLine = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressLine, 400);
            Number = ConverterValidatorsExtensions.StringLengthValidation(dto.Number, 50);
            Neighbourhood = ConverterValidatorsExtensions.StringLengthValidation(dto.Neighbourhood, 250);
            City = ConverterValidatorsExtensions.StringLengthValidation(dto.City, 250);
            State = ConverterValidatorsExtensions.StringLengthValidation(dto.State, 50);
            PostalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.PostalCode, 20);
            AddressNotes = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressNotes, 400);
            Landmark = ConverterValidatorsExtensions.StringLengthValidation(dto.Landmark, 400);
            Latitude = NullableConverters.ToNullableDecimal(dto.Latitude);
            Longitude = NullableConverters.ToNullableDecimal(dto.Longitude);
        }
    }
}
