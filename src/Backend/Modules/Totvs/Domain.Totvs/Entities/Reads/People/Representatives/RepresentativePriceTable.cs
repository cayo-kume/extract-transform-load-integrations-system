using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Representatives
{
    /// <summary>
    /// tabela de preço do representante (RepresentativePriceTableModel), tabela totvs.TotvsRepresentativesPriceTables. personCode é o código do representante (uma pessoa do PESFM010)
    /// </summary>
    public class RepresentativePriceTable
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int priceTableCode { get; private set; }
        public string name { get; private set; } = String.Empty;

        public RepresentativePriceTable() { }

        public RepresentativePriceTable(string doc_company, DateTime integrated_or_updated_at, int personCode, Dtos.Reads.People.Representatives.RepresentativePriceTable dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            priceTableCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
        }
    }
}
