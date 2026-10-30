using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsPersonCatalogsCommandHandler : ITotvsPersonCatalogsCommandHandler
    {
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
    }
}
