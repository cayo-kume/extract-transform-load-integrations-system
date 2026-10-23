using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// item da instrução de conservação, tabela totvs.TotvsProductsConservationInstructionsItems
    /// </summary>
    public class ConservationInstructionItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int instructionCode { get; private set; }
        public int order { get; private set; }
        public int item { get; private set; }

        public ConservationInstructionItem() { }

        public ConservationInstructionItem(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, int instructionCode, Dtos.Reads.Products.ConservationInstructionItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;
            this.instructionCode = instructionCode;

            order = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.order);
            item = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.item);
        }
    }
}
