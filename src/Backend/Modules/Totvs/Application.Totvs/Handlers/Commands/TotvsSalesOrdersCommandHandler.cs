using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsSalesOrdersCommandHandler : ITotvsSalesOrdersCommandHandler
    {
        public string CreateGetSalesOrdersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSALEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        /// <summary>
        /// pedidos por (loja, número): o número só é único por loja. VALUES aceita até 1000 linhas, o tamanho do lote do repositório;
        /// com lista vazia a query é inválida, e o repositório não a chama nesse caso
        /// </summary>
        public string CreateGetSalesOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys)
        {
            return @$"SELECT O.DOC_COMPANY, O.BRANCHCODE, O.ORDERCODE, O.MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSALEORDERS O
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.orderCode})"))}) K (BRANCHCODE, ORDERCODE)
                     ON K.BRANCHCODE = O.BRANCHCODE AND K.ORDERCODE = O.ORDERCODE
                     WHERE O.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleDiscountTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSALEDISCOUNTTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleDiscountTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSALEDISCOUNTTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleClassificationsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSALECLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleClassificationsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, TYPECODE, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSALECLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleOrderBatchesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSALEORDERBATCHES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleOrderBatchesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSALEORDERBATCHES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBillingSuggestionsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSBILLINGSUGGESTIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBillingSuggestionsByKeysQuery(string doc_company, List<(int branchCode, int suggestionCode)> keys)
        {
            // sugestões por (loja, código), como os pedidos; com lista vazia a query é inválida, e o repositório não a chama nesse caso
            return @$"SELECT S.DOC_COMPANY, S.BRANCHCODE, S.SUGGESTIONCODE, S.MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSBILLINGSUGGESTIONS S
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.suggestionCode})"))}) K (BRANCHCODE, SUGGESTIONCODE)
                     ON K.BRANCHCODE = S.BRANCHCODE AND K.SUGGESTIONCODE = S.SUGGESTIONCODE
                     WHERE S.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSaleOrderStatusQuery(string doc_company, int branchCode, int orderCode)
        {
            // situação gravada pela leitura (nome do StatusOrderType); sem linha, o pedido ainda não chegou ao banco
            return @$"SELECT STATUSORDER
                     FROM TOTVS.TOTVSSALEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND ORDERCODE = {orderCode}";
        }

        public string CreateGetSaleOrderProductsQuery(string doc_company, int branchCode, int orderCode)
        {
            return @$"SELECT PRODUCTCODE AS productCode, PRODUCTSKU AS productSku, [QUANTITY] AS quantity, PENDINGQUANTITY AS pendingQuantity
                     FROM TOTVS.TOTVSSALEORDERSPRODUCTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND ORDERCODE = {orderCode}";
        }
    }
}
