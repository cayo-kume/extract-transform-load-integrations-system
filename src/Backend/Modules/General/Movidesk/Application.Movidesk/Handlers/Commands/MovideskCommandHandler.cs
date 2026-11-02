using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskCommandHandler : IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery(string company)
        {
            // só os CNPJs da holding da execução (monitor.HoldingCompanies, docs/PLANO-TELA-CADASTRO-EMPRESA.md 4.4); company é só
            // dígitos (ICompanyContext)
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     TOKEN AS TOKEN
                     FROM MOVIDESK.MOVIDESKPARAMETERS P
                     JOIN MONITOR.HOLDINGCOMPANIES HC ON HC.COMPANY = P.DOC_COMPANY
                     WHERE HC.HOLDING = '{company}'";
        }

        public string CreateGetCompanyQuery(string doc_company)
        {
            // doc_company já passou pelo ^\d{14}$ do controller; com aspas porque a coluna é nvarchar (CNPJ pode começar com zero)
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     TOKEN AS TOKEN
                     FROM MOVIDESK.MOVIDESKPARAMETERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
