using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskCommandHandler : IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     TOKEN AS TOKEN
                     FROM MOVIDESK.MOVIDESKPARAMETERS";
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
