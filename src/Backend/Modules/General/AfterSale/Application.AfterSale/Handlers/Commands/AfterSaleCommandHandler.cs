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

        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                    DOC_COMPANY,
                    TOKEN AS TOKEN
                    FROM AFTERSALE.AFTERSALEPARAMETERS
                    --WHERE DOC_COMPANY = '38367316000199'";
        }
    }
}
