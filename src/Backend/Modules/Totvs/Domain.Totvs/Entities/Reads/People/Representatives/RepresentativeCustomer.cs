using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Representatives
{
    /// <summary>
    /// cliente da carteira do representante (ViewRepresentativeCustomerModel), tabela totvs.TotvsRepresentativesCustomers. personCode é o código do representante (uma pessoa do PESFM010)
    /// </summary>
    public class RepresentativeCustomer
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int customerCode { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string cpfCnpj { get; private set; } = String.Empty;

        public RepresentativeCustomer() { }

        public RepresentativeCustomer(string doc_company, DateTime integrated_or_updated_at, int personCode, Dtos.Reads.People.Representatives.RepresentativeCustomer dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            customerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
        }
    }
}
