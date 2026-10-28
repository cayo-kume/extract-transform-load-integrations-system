using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// sugestão de faturamento, tabela totvs.TotvsBillingSuggestions, com os pedidos e os itens
    /// </summary>
    public class BillingSuggestion
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int suggestionCode { get; private set; }
        public int? statusSuggestion { get; private set; }
        public DateTime? suggestionDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<BillingSuggestionOrder> orders { get; private set; } = [];

        [SkipProperty]
        public List<BillingSuggestionOrderItem> orderItems { get; private set; } = [];

        public BillingSuggestion() { }

        public BillingSuggestion(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestion dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            suggestionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.suggestionCode);
            statusSuggestion = NullableConverters.ToNullableInt32(dto.statusSuggestion);
            suggestionDate = NullableConverters.ToNullableDateTime(dto.suggestionDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            foreach (var order in (dto.orders ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.orderBranchCode) != 0 && ConverterValidatorsExtensions.ConvertToInt32Validation(x.orderCode) != 0).GroupBy(x => (x.orderBranchCode, x.orderCode)).Select(g => g.Last()))
            {
                var entity = new BillingSuggestionOrder(doc_company, integrated_or_updated_at, branchCode, suggestionCode, order);
                orders.Add(entity);

                orderItems.AddRange((order.orderItems ?? [])
                    .Select(x => new BillingSuggestionOrderItem(doc_company, integrated_or_updated_at, branchCode, suggestionCode, entity.orderBranchCode, entity.orderCode, x))
                    .Where(x => x.productCode != 0)
                    .GroupBy(x => x.productCode)
                    .Select(g => g.Last()));
            }
        }
    }
}
