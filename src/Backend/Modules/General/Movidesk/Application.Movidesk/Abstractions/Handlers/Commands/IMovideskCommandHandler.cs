namespace Application.Movidesk.Abstractions.Handlers.Commands
{
    public interface IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery(string company);
        public string CreateGetCompanyQuery(string doc_company);
    }
}
