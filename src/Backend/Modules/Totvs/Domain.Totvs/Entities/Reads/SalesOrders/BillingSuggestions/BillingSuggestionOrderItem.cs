using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// item do pedido na sugestão de faturamento, tabela totvs.TotvsBillingSuggestionsOrdersItems
    /// </summary>
    public class BillingSuggestionOrderItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int suggestionCode { get; private set; }
        public int orderBranchCode { get; private set; }
        public int orderCode { get; private set; }
        public int productCode { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string standardBarCode { get; private set; } = String.Empty;
        public string sizeName { get; private set; } = String.Empty;
        public string colorName { get; private set; } = String.Empty;
        public decimal? quantity { get; private set; }
        public decimal? suggestedQuantity { get; private set; }
        public decimal? pendingQuantity { get; private set; }
        public decimal? price { get; private set; }

        public BillingSuggestionOrderItem() { }

        public BillingSuggestionOrderItem(string doc_company, DateTime integrated_or_updated_at, int branchCode, int suggestionCode, int orderBranchCode, int orderCode, Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestionOrderItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.suggestionCode = suggestionCode;
            this.orderBranchCode = orderBranchCode;
            this.orderCode = orderCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            standardBarCode = ConverterValidatorsExtensions.StringLengthValidation(dto.standardBarCode, 40);
            sizeName = ConverterValidatorsExtensions.StringLengthValidation(dto.sizeName, 10);
            colorName = ConverterValidatorsExtensions.StringLengthValidation(dto.colorName, 40);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            suggestedQuantity = NullableConverters.ToNullableDecimal(dto.suggestedQuantity);
            pendingQuantity = NullableConverters.ToNullableDecimal(dto.pendingQuantity);
            price = NullableConverters.ToNullableDecimal(dto.price);
        }
    }
}
