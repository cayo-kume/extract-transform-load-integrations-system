using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsProductCatalogsCommandHandler : ITotvsProductCatalogsCommandHandler
    {
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
    }
}
