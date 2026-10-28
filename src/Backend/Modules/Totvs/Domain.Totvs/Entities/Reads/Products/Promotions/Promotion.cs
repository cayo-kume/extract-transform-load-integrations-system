using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Promotions
{
    /// <summary>
    /// item de promoção (promoção x produto) de uma loja, tabela totvs.TotvsPromotions. O filtro de data da rota dá erro na Totvs
    /// (ORA-00920, testado): a loja é lida inteira e json_hash (SHA-256 do JSON do item) diz se ele mudou (T9 do
    /// docs/PLANO-TOTVS-METODOS-FALTANTES.md), o que pega também a troca de valor e de situação (PR15)
    /// </summary>
    public class Promotion
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int promotionCode { get; private set; }
        public int productCode { get; private set; }
        public string promotionDescription { get; private set; } = String.Empty;
        public DateTime? startDate { get; private set; }
        public DateTime? endDate { get; private set; }
        public decimal? previousValue { get; private set; }
        public decimal? promotionValue { get; private set; }
        public string status { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }
        public string json_hash { get; private set; } = String.Empty;

        public Promotion() { }

        public Promotion(string doc_company, DateTime integrated_or_updated_at, string json_hash, Dtos.Reads.Products.Promotions.Promotion dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            promotionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.promotionCode);
            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            promotionDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.promotionDescription, 40);
            startDate = NullableConverters.ToNullableDateTime(dto.startDate);
            endDate = NullableConverters.ToNullableDateTime(dto.endDate);
            previousValue = NullableConverters.ToNullableDecimal(dto.previousValue);
            promotionValue = NullableConverters.ToNullableDecimal(dto.promotionValue);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 10);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
