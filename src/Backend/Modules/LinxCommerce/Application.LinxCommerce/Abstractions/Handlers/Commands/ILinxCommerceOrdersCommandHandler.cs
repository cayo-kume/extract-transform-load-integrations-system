namespace Application.LinxCommerce.Abstractions.Handlers.Commands
{
    public interface ILinxCommerceOrdersCommandHandler
    {
        public string CreateGetOrdersLastModifiedDateQuery(string doc_company);
        public string CreateGetOrdersByIdsQuery(string doc_company, List<Guid> ids);
    }
}
