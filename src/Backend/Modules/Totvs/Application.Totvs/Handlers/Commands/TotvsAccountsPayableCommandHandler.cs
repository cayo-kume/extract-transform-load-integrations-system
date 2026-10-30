using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsAccountsPayableCommandHandler : ITotvsAccountsPayableCommandHandler
    {
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
    }
}
