using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// pedido da sugestão de faturamento, tabela totvs.TotvsBillingSuggestionsOrders
    /// </summary>
    public class BillingSuggestionOrder
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int suggestionCode { get; private set; }
        public int orderBranchCode { get; private set; }
        public int orderCode { get; private set; }
        public decimal? quantity { get; private set; }
        public decimal? suggestedQuantity { get; private set; }
        public decimal? pendingQuantity { get; private set; }
        public decimal? value { get; private set; }
        public decimal? suggestedValue { get; private set; }
        public decimal? pendingValue { get; private set; }

        public BillingSuggestionOrder() { }

        public BillingSuggestionOrder(string doc_company, DateTime integrated_or_updated_at, int branchCode, int suggestionCode, Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestionOrder dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.suggestionCode = suggestionCode;

            orderBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.orderBranchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.orderCode);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            suggestedQuantity = NullableConverters.ToNullableDecimal(dto.suggestedQuantity);
            pendingQuantity = NullableConverters.ToNullableDecimal(dto.pendingQuantity);
            value = NullableConverters.ToNullableDecimal(dto.value);
            suggestedValue = NullableConverters.ToNullableDecimal(dto.suggestedValue);
            pendingValue = NullableConverters.ToNullableDecimal(dto.pendingValue);
        }
    }
}
