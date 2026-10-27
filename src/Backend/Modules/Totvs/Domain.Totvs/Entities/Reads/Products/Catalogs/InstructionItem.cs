using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// item de instrução (GET product/v2/instruction-items, InstructionItemResultModel; sem as imagens, PR13), tabela totvs.TotvsInstructionItems
    /// </summary>
    public class InstructionItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public int? grouper { get; private set; }
        public string grouperDescription { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public InstructionItem() { }

        public InstructionItem(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.InstructionItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 200);
            grouper = NullableConverters.ToNullableInt32(dto.grouper);
            grouperDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.grouperDescription, 40);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
