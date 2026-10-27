using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsCommandHandler : ITotvsCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     CLIENT_ID AS CLIENT_ID,
                     CLIENT_SECRET AS CLIENT_SECRET,
                     USERNAME AS USERNAME,
                     [PASSWORD] AS [PASSWORD]
                     FROM TOTVS.TOTVSPARAMETERS";
        }

        public string CreateGetBranchCodesQuery(string doc_company)
        {
            // lojas da holding usadas no branchCodeList das buscas (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 4.2)
            return @$"SELECT CODE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     ORDER BY CODE";
        }

        public string CreateGetBrandBranchCodesQuery(string doc_company, int baseBranchCode)
        {
            // lojas da marca: mesma raiz de CNPJ (8 primeiros dígitos) da matriz usada como empresa base dos produtos
            return @$"SELECT B.CODE
                     FROM TOTVS.TOTVSBRANCHES B
                     INNER JOIN TOTVS.TOTVSBRANCHES M ON M.DOC_COMPANY = B.DOC_COMPANY AND M.CODE = {baseBranchCode}
                     WHERE B.DOC_COMPANY = '{doc_company}'
                     AND LEFT(B.CNPJ, 8) = LEFT(M.CNPJ, 8)
                     ORDER BY B.CODE";
        }

        public string CreateGetBranchCodeByCnpjQuery(string doc_company, string cnpj)
        {
            // loja da empresa pelo CNPJ (escritas: o corpo da Totvs identifica a loja pelo CNPJ). O cnpj chega validado (14 dígitos)
            return @$"SELECT CODE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CNPJ = '{cnpj}'";
        }

        public string CreateGetBranchesLastChangeQuery(string doc_company)
        {
            // doc_company vem de TotvsParameters; com aspas porque a coluna é nvarchar (CNPJ pode começar com zero)
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBranchesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetCustomersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCustomersByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetLegalEntitiesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSLEGALENTITIES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetLegalEntitiesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSLEGALENTITIES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetSellersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSELLERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSellersByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, SELLERCODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSELLERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND SELLERCODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetOperationalAreasHashesQuery(string doc_company)
        {
            // regiões gravadas da empresa, com o hash da região agrupada (a resposta da API não tem data de alteração sem filtro)
            return @$"SELECT DOC_COMPANY, OPERATIONALAREACODE, JSON_HASH
                     FROM TOTVS.TOTVSOPERATIONALAREAS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPurchaseOrdersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPURCHASEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPurchaseOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys) =>
            CreateGetOrdersByKeysQuery("TOTVSPURCHASEORDERS", doc_company, keys);

        public string CreateGetSalesOrdersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSALEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSalesOrdersByKeysQuery(string doc_company, List<(int branchCode, int orderCode)> keys) =>
            CreateGetOrdersByKeysQuery("TOTVSSALEORDERS", doc_company, keys);

        /// <summary>
        /// pedidos por (loja, número): o número só é único por loja. VALUES aceita até 1000 linhas, o tamanho do lote do repositório;
        /// com lista vazia a query é inválida, e o repositório não a chama nesse caso
        /// </summary>
        private static string CreateGetOrdersByKeysQuery(string table, string doc_company, List<(int branchCode, int orderCode)> keys)
        {
            return @$"SELECT O.DOC_COMPANY, O.BRANCHCODE, O.ORDERCODE, O.MAXCHANGEFILTERDATE
                     FROM TOTVS.{table} O
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.orderCode})"))}) K (BRANCHCODE, ORDERCODE)
                     ON K.BRANCHCODE = O.BRANCHCODE AND K.ORDERCODE = O.ORDERCODE
                     WHERE O.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetDuplicatesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSDUPLICATES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetDuplicatesByKeysQuery(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys)
        {
            // a duplicata é uma parcela: loja + fornecedor + duplicata + parcela. VALUES aceita até 1000 linhas, o tamanho do lote do
            // repositório; com lista vazia a query é inválida, e o repositório não a chama nesse caso
            return @$"SELECT D.DOC_COMPANY, D.BRANCHCODE, D.SUPPLIERCODE, D.DUPLICATECODE, D.INSTALLMENTCODE, D.MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSDUPLICATES D
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.supplierCode}, {k.duplicateCode}, {k.installmentCode})"))}) K (BRANCHCODE, SUPPLIERCODE, DUPLICATECODE, INSTALLMENTCODE)
                     ON K.BRANCHCODE = D.BRANCHCODE AND K.SUPPLIERCODE = D.SUPPLIERCODE AND K.DUPLICATECODE = D.DUPLICATECODE AND K.INSTALLMENTCODE = D.INSTALLMENTCODE
                     WHERE D.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetInvoicesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSINVOICES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetInvoicesByKeysQuery(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys)
        {
            // a nota é identificada por loja + sequência + data (5.2.1). VALUES aceita até 1000 linhas, o tamanho do lote do
            // repositório; com lista vazia a query é inválida, e o repositório não a chama nesse caso. Data no estilo 126 (ISO)
            return @$"SELECT N.DOC_COMPANY, N.BRANCHCODE, N.INVOICESEQUENCE, N.INVOICEDATE, N.MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSINVOICES N
                     INNER JOIN (VALUES {String.Join(", ", keys.Select(k => $"({k.branchCode}, {k.invoiceSequence}, CONVERT(DATETIME, '{k.invoiceDate:yyyy-MM-ddTHH:mm:ss.fff}', 126))"))}) K (BRANCHCODE, INVOICESEQUENCE, INVOICEDATE)
                     ON K.BRANCHCODE = N.BRANCHCODE AND K.INVOICESEQUENCE = N.INVOICESEQUENCE AND K.INVOICEDATE = N.INVOICEDATE
                     WHERE N.DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPersonClassificationsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPERSONCLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPersonClassificationsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, TYPECODE, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPERSONCLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetEmailTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSEMAILTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetEmailTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSEMAILTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPhoneTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPHONETYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPhoneTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPHONETYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetContactTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSCONTACTTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetContactTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSCONTACTTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetRepresentativesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSREPRESENTATIVES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetRepresentativesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSREPRESENTATIVES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetReceptionSalesLastMovementDateQuery(string doc_company)
        {
            return @$"SELECT MAX(MOVEMENTDATE)
                     FROM TOTVS.TOTVSRECEPTIONSALES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetReceptionSalesHashesQuery(string doc_company, DateTime startMovementDate, DateTime endMovementDate)
        {
            // recepções gravadas da fatia, com o hash do JSON (a resposta da API não tem data de alteração). Datas no estilo 126 (ISO)
            return @$"SELECT DOC_COMPANY, BRANCHCODE, MOVEMENTDATE, SEQUENCE, JSON_HASH
                     FROM TOTVS.TOTVSRECEPTIONSALES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND MOVEMENTDATE >= CONVERT(DATETIME, '{startMovementDate:yyyy-MM-dd}T00:00:00', 126)
                     AND MOVEMENTDATE < CONVERT(DATETIME, '{endMovementDate.AddDays(1):yyyy-MM-dd}T00:00:00', 126)";
        }

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

        public string CreateGetClassificationTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSCLASSIFICATIONTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetClassificationTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSCLASSIFICATIONTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductClassificationsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPRODUCTCLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductClassificationsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, TYPECODE, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRODUCTCLASSIFICATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCategoriesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSCATEGORIES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCategoriesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSCATEGORIES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetGridsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSGRIDS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetGridsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSGRIDS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetMeasurementUnitsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSMEASUREMENTUNITS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetMeasurementUnitsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSMEASUREMENTUNITS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetColorsHashesQuery(string doc_company)
        {
            // cores gravadas da empresa, com o hash do JSON (sem filtro de data a resposta não tem data de alteração)
            return @$"SELECT DOC_COMPANY, COLORCODE, JSON_HASH
                     FROM TOTVS.TOTVSCOLORS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetAdditionalFieldsTypesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSADDITIONALFIELDSTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetAdditionalFieldsTypesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSADDITIONALFIELDSTYPES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductGrouperConfigurationsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPRODUCTGROUPERCONFIGURATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductGrouperConfigurationsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, GROUPERCODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRODUCTGROUPERCONFIGURATIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetInstructionItemsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSINSTRUCTIONITEMS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetInstructionItemsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSINSTRUCTIONITEMS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCompositionsLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSCOMPOSITIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCompositionsQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSCOMPOSITIONS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPriceTablesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPRICETABLES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPriceTablesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRICETABLES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPriceTableScalesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPRICETABLESCALES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPriceTableScalesQuery(string doc_company)
        {
            // catálogo pequeno: o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, SCALECODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPRICETABLESCALES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

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
    }
}
