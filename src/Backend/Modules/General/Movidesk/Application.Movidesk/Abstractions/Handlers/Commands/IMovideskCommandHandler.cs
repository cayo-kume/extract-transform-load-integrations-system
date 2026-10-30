namespace Application.Movidesk.Abstractions.Handlers.Commands
{
    public interface IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetCompanyQuery(string doc_company);
    }
}
