using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// instrução de conservação do produto, tabela totvs.TotvsProductsConservationInstructions (tamanhos do swagger)
    /// </summary>
    public class ConservationInstruction
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public bool? isDefault { get; private set; }   // "default" na API
        public int grouperCode { get; private set; }

        [SkipProperty]
        public List<ConservationInstructionItem> items { get; private set; } = [];

        public ConservationInstruction() { }

        public ConservationInstruction(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.ConservationInstruction dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 80);
            isDefault = NullableConverters.ToNullableBoolean(dto.@default);
            grouperCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.grouperCode);

            items.AddRange((dto.items ?? []).Select(x => new ConservationInstructionItem(doc_company, integrated_or_updated_at, branchInfoCode, productCode, code, x)).GroupBy(x => x.order).Select(g => g.Last()));
        }
    }
}
