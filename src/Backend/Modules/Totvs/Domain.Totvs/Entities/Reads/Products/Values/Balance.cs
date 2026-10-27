using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// saldo de um produto numa loja, por tipo de saldo (PRDFL002), tabela totvs.TotvsBalances
    /// </summary>
    public class Balance
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int productCode { get; private set; }
        public int stockCode { get; private set; }
        public string stockDescription { get; private set; } = String.Empty;
        public decimal? stock { get; private set; }
        public decimal? salesOrder { get; private set; }
        public decimal? inputTransaction { get; private set; }
        public decimal? outputTransaction { get; private set; }
        public decimal? productionPlanning { get; private set; }
        public decimal? purchaseOrder { get; private set; }
        public decimal? productionOrderProgress { get; private set; }
        public decimal? productionOrderWaitLib { get; private set; }
        public decimal? stockTemp { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public Balance() { }

        public Balance(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Values.ProductBalance product, Dtos.Reads.Products.Values.ProductBalanceItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(product.productCode);
            stockCode = (int)(NullableConverters.ToNullableDecimal(dto.stockCode) ?? 0);
            stockDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.stockDescription, 60);
            stock = NullableConverters.ToNullableDecimal(dto.stock);
            salesOrder = NullableConverters.ToNullableDecimal(dto.salesOrder);
            inputTransaction = NullableConverters.ToNullableDecimal(dto.inputTransaction);
            outputTransaction = NullableConverters.ToNullableDecimal(dto.outputTransaction);
            productionPlanning = NullableConverters.ToNullableDecimal(dto.productionPlanning);
            purchaseOrder = NullableConverters.ToNullableDecimal(dto.purchaseOrder);
            productionOrderProgress = NullableConverters.ToNullableDecimal(dto.productionOrderProgress);
            productionOrderWaitLib = NullableConverters.ToNullableDecimal(dto.productionOrderWaitLib);
            stockTemp = NullableConverters.ToNullableDecimal(dto.stockTemp);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(product.maxChangeFilterDate);
        }
    }
}
