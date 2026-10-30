namespace Application.Movidesk.Abstractions.Handlers.Commands
{
    public interface IMovideskPeopleCommandHandler
    {
        public string CreateGetPeopleLastChangeQuery(string doc_company);
        public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids);
        public string CreateDeletePersonQuery();
    }
}
