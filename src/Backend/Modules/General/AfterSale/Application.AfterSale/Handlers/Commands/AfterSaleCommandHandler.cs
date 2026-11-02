using Application.AfterSale.Abstractions.Handlers.Commands;

namespace Application.AfterSale.Handlers.Commands
{
    public class AfterSaleCommandHandler : IAfterSaleCommandHandler
    {
        public string CreateGetCompanyQuery(string doc_company)
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY,
                     TOKEN
                     FROM AFTERSALE.AFTERSALEPARAMETERS
                     WHERE 
                     DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCompanysQuery(string company)
        {
            // só os CNPJs da holding da execução (monitor.HoldingCompanies, docs/PLANO-TELA-CADASTRO-EMPRESA.md 4.4); company é só
            // dígitos (ICompanyContext)
            return @$"SELECT DISTINCT
                    DOC_COMPANY,
                    TOKEN AS TOKEN
                    FROM AFTERSALE.AFTERSALEPARAMETERS P
                     JOIN MONITOR.HOLDINGCOMPANIES HC ON HC.COMPANY = P.DOC_COMPANY
                     WHERE HC.HOLDING = '{company}'";
        }
    }
}
