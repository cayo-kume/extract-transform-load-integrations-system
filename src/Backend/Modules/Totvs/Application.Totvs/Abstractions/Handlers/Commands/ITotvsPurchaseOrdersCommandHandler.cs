namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsPurchaseOrdersCommandHandler
    {
        public string CreateGetPurchaseOrdersLastChangeQuery(string doc_company);
        public string CreateGetPurchaseOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys);

        public string CreateGetPurchaseOrderStatusQuery(string doc_company, int branchCode, int orderCode);
        public string CreateGetPurchaseOrderStateQuery(string doc_company, int branchCode, int orderCode);
        public string CreateGetPurchaseOrderBySupplierOrderCodeQuery(string doc_company, int branchCode, int? supplierCode, string? supplierCpfCnpj, string supplierOrderCode);
        public string CreateGetPurchaseOrderProductsQuery(string doc_company, int branchCode, int orderCode);
    }
}
