using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// composição do cadastro mestre (POST product/v2/compositions, CompositionResultModel), tabela totvs.TotvsCompositions e filhas
    /// </summary>
    public class Composition
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<CompositionFiber> fibers { get; private set; } = [];

        public Composition() { }

        public Composition(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.Composition dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 80);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            fibers.AddRange((dto.fibers ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).Select(x => new CompositionFiber(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
        }
    }
}
