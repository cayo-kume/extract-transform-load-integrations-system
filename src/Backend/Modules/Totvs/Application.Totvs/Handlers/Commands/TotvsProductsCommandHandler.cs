using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsProductsCommandHandler : ITotvsProductsCommandHandler
    {
        public string CreateGetSkusLastChangeQuery(string doc_company, int branchInfoCode)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSKUS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHINFOCODE = {branchInfoCode}";
        }

        public string CreateGetSkusByIdsQuery(string doc_company, int branchInfoCode, List<int> referenceIds)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, BRANCHINFOCODE, REFERENCEID, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSKUS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHINFOCODE = {branchInfoCode}
                     AND REFERENCEID IN ({String.Join(", ", referenceIds)})";
        }

        public string CreateGetBalancesStateQuery(string doc_company, int branchCode)
        {
            // estado da loja: maior alteração (null só com a carga inicial), maior produto (onde a carga inicial continua) e começo da carga
            return @$"SELECT MAX(MAXCHANGEFILTERDATE) AS LASTCHANGE, MAX(PRODUCTCODE) AS MAXPRODUCTCODE, MIN(INTEGRATED_OR_UPDATED_AT) AS FIRSTINTEGRATED
                     FROM TOTVS.TOTVSBALANCES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}";
        }

        public string CreateGetBalancesChangesQuery(string doc_company, int branchCode, List<int> productCodes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT PRODUCTCODE, MAX(MAXCHANGEFILTERDATE) AS MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSBALANCES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}
                     AND PRODUCTCODE IN ({String.Join(", ", productCodes)})
                     GROUP BY PRODUCTCODE";
        }

        public string CreateGetSimilarHashesQuery(string doc_company)
        {
            // produtos gravados com o hash do JSON (lastChageDate vem 0001-01-01: o hash diz o que mudou)
            return @$"SELECT PRODUCTCODE, JSON_HASH
                     FROM TOTVS.TOTVSPRODUCTSSIMILAR
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSimilarLastWindowEndQuery(string doc_company)
        {
            // fim da última janela lida e gravada (as janelas são lidas em ordem de data e gravadas só inteiras)
            return @$"SELECT MAX(CHANGEWINDOWEND)
                     FROM TOTVS.TOTVSPRODUCTSSIMILAR
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBatchesHashesQuery(string doc_company, int branchCode)
        {
            // lotes gravados da loja com o hash do JSON (a resposta não tem data de alteração: a leitura é inteira)
            return @$"SELECT BRANCHCODE, BATCHCODE, PRODUCTCODE, JSON_HASH
                     FROM TOTVS.TOTVSBATCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHCODE = {branchCode}";
        }

        public string CreateGetSkusProductCodesQuery(string doc_company, int branchInfoCode)
        {
            // produtos gravados das referências da matriz, para a conferência do product-codes/search
            return @$"SELECT [CODE]
                     FROM TOTVS.TOTVSSKUSCOLORSPRODUCTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND BRANCHINFOCODE = {branchInfoCode}";
        }
    }
}
