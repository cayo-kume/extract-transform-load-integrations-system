using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// fibra da composição (FiberResultModel), tabela totvs.TotvsCompositionsFibers
    /// </summary>
    public class CompositionFiber
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int compositionCode { get; private set; }
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public decimal? percentage { get; private set; }

        public CompositionFiber() { }

        public CompositionFiber(string doc_company, DateTime integrated_or_updated_at, int compositionCode, Dtos.Reads.Products.Catalogs.CompositionFiber dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.compositionCode = compositionCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            percentage = NullableConverters.ToNullableDecimal(dto.percentage);
        }
    }
}
