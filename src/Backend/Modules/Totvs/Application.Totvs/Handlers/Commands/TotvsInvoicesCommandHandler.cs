using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsInvoicesCommandHandler : ITotvsInvoicesCommandHandler
    {
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

        public string CreateGetInvoicesDisableLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSINVOICESDISABLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetInvoicesDisableQuery(string doc_company)
        {
            // poucas faixas por ano (~120 em 12 meses, testado): o cache de estado é montado com a tabela inteira da empresa
            return @$"SELECT DOC_COMPANY, BRANCHCODE, DOCUMENTTYPE, SERIALCODE, INITIALINVOICENUMBER, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSINVOICESDISABLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCostCentersHashesQuery(string doc_company)
        {
            // centros gravados com o hash do JSON (a resposta não tem data de alteração: a leitura é inteira)
            return @$"SELECT [CODE], JSON_HASH
                     FROM TOTVS.TOTVSCOSTCENTERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
