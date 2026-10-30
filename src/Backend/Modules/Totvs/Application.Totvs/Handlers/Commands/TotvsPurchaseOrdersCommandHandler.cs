using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsPurchaseOrdersCommandHandler : ITotvsPurchaseOrdersCommandHandler
    {
        public string CreateGetPurchaseOrdersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPURCHASEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        /// <summary>
        /// pedidos por (loja, número): o número só é único por loja. VALUES aceita até 1000 linhas, o tamanho do lote do repositório;
        /// com lista vazia a query é inválida, e o repositório não a chama nesse caso
        /// </summary>
        public string CreateGetPurchaseOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys)
        {
            return @$"SELECT O.DOC_COMPANY, O.BRANCHCODE, O.ORDERCODE, O.MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPURCHASEORDERS O
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.orderCode})"))}) K (BRANCHCODE, ORDERCODE)
                     ON K.BRANCHCODE = O.BRANCHCODE AND K.ORDERCODE = O.ORDERCODE
                     WHERE O.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPurchaseOrderStatusQuery(string doc_company, int branchCode, int orderCode)
        {
            // situação gravada pela leitura (nome do StatusOrderType); sem linha, o pedido ainda não chegou ao banco
            return @$"SELECT [STATUS]
                     FROM TOTVS.TOTVSPURCHASEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND ORDERCODE = {orderCode}";
        }

        public string CreateGetPurchaseOrderStateQuery(string doc_company, int branchCode, int orderCode)
        {
            return @$"SELECT [STATUS] AS status, SUPPLIERCODE AS supplierCode, SUPPLIERCPFCNPJ AS supplierCpfCnpj
                     FROM TOTVS.TOTVSPURCHASEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND ORDERCODE = {orderCode}";
        }

        public string CreateGetPurchaseOrderBySupplierOrderCodeQuery(string doc_company, int branchCode, int? supplierCode, string? supplierCpfCnpj, string supplierOrderCode)
        {
            // pedido já gravado com o mesmo pedido do fornecedor (DP2); o texto vem do corpo, com as aspas dobradas
            var supplier = supplierCode is not null
                ? $"SUPPLIERCODE = {supplierCode}"
                : $"SUPPLIERCPFCNPJ = N'{supplierCpfCnpj!.Replace("'", "''")}'";

            return @$"SELECT TOP 1 ORDERCODE
                     FROM TOTVS.TOTVSPURCHASEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND {supplier}
                     AND SUPPLIERORDERCODE = N'{supplierOrderCode.Replace("'", "''")}'
                     ORDER BY ORDERCODE DESC";
        }

        public string CreateGetPurchaseOrderProductsQuery(string doc_company, int branchCode, int orderCode)
        {
            return @$"SELECT PRODUCTCODE AS productCode, SETTLEDQUANTITY AS settledQuantity
                     FROM TOTVS.TOTVSPURCHASEORDERSPRODUCTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND ORDERCODE = {orderCode}";
        }
    }
}
