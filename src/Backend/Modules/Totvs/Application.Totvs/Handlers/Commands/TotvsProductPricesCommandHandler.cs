using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsProductPricesCommandHandler : ITotvsProductPricesCommandHandler
    {
        public string CreateGetPricesStateQuery(string doc_company, int branchCode)
        {
            // estado da loja: maior alteração (null só com a carga inicial), maior produto (onde a carga inicial continua) e começo da carga
            return @$"SELECT MAX(MAXCHANGEFILTERDATE) AS LASTCHANGE, MAX(PRODUCTCODE) AS MAXPRODUCTCODE, MIN(INTEGRATED_OR_UPDATED_AT) AS FIRSTINTEGRATED
                     FROM TOTVS.TOTVSPRICES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}";
        }

        public string CreateGetPricesChangesQuery(string doc_company, int branchCode, List<int> productCodes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT PRODUCTCODE, MAX(MAXCHANGEFILTERDATE) AS MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRICES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND PRODUCTCODE IN ({String.Join(", ", productCodes)})
                     GROUP BY PRODUCTCODE";
        }

        public string CreateGetCostsStateQuery(string doc_company, int branchCode)
        {
            // estado da loja: maior alteração (null só com a carga inicial), maior produto (onde a carga inicial continua) e começo da carga
            return @$"SELECT MAX(MAXCHANGEFILTERDATE) AS LASTCHANGE, MAX(PRODUCTCODE) AS MAXPRODUCTCODE, MIN(INTEGRATED_OR_UPDATED_AT) AS FIRSTINTEGRATED
                     FROM TOTVS.TOTVSCOSTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}";
        }

        public string CreateGetCostsChangesQuery(string doc_company, int branchCode, List<int> productCodes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT PRODUCTCODE, MAX(MAXCHANGEFILTERDATE) AS MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSCOSTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND PRODUCTCODE IN ({String.Join(", ", productCodes)})
                     GROUP BY PRODUCTCODE";
        }

        public string CreateGetActivePriceTableCodesQuery(string doc_company)
        {
            // tabelas de preço vigentes do cabeçalho (GetPriceTablesHeaders): sem período final ou com ele de hoje em diante
            return @$"SELECT CODE
                     FROM TOTVS.TOTVSPRICETABLES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND (ENDDATE IS NULL OR ENDDATE >= CAST(GETDATE() AS DATE))
                     ORDER BY CODE";
        }

        public string CreateGetPriceTableItemsStateQuery(string doc_company, int priceTableCode)
        {
            // estado da tabela: maior alteração (null só com a carga inicial), maior produto (onde a carga inicial continua) e começo da carga
            return @$"SELECT MAX(MAXCHANGEFILTERDATE) AS LASTCHANGE, MAX(PRODUCTCODE) AS MAXPRODUCTCODE, MIN(INTEGRATED_OR_UPDATED_AT) AS FIRSTINTEGRATED
                     FROM TOTVS.TOTVSPRICETABLESITEMS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRICETABLECODE = {priceTableCode}";
        }

        public string CreateGetPriceTableItemsChangesQuery(string doc_company, int priceTableCode, List<int> productCodes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT PRODUCTCODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRICETABLESITEMS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRICETABLECODE = {priceTableCode}
                     AND PRODUCTCODE IN ({String.Join(", ", productCodes)})";
        }

        public string CreateGetPromotionsHashesQuery(string doc_company, int branchCode)
        {
            // itens de promoção gravados da loja, com o hash do JSON (o filtro de data da rota dá erro na Totvs: a loja é lida inteira)
            return @$"SELECT BRANCHCODE, PROMOTIONCODE, PRODUCTCODE, JSON_HASH
                     FROM TOTVS.TOTVSPROMOTIONS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}";
        }

        public string CreateGetCurrentProductValuesQuery(string doc_company, List<int> productCodes)
        {
            // preço e custo gravados pelas leituras da fase 3, no mesmo formato da escrita (P/C + código)
            var codes = String.Join(",", productCodes);

            return @$"SELECT BRANCHCODE AS branchCode, PRODUCTCODE AS productCode, 'P' AS valueType, PRICECODE AS valueCode, [PRICE] AS value
                     FROM TOTVS.TOTVSPRICES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRODUCTCODE IN ({codes})
                     UNION ALL
                     SELECT BRANCHCODE, PRODUCTCODE, 'C', COSTCODE, [COST]
                     FROM TOTVS.TOTVSCOSTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRODUCTCODE IN ({codes})";
        }

        public string CreateGetPromotionItemsQuery(string doc_company, int branchCode, int promotionCode)
        {
            return @$"SELECT PRODUCTCODE AS productCode, PREVIOUSVALUE AS previousValue, [STATUS] AS status
                     FROM TOTVS.TOTVSPROMOTIONS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND PROMOTIONCODE = {promotionCode}";
        }
    }
}
