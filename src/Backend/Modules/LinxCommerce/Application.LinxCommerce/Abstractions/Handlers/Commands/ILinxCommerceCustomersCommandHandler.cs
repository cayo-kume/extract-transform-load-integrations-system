namespace Application.LinxCommerce.Abstractions.Handlers.Commands
{
    public interface ILinxCommerceCustomersCommandHandler
    {
        public string CreateGetCustomersLastBatchTimeQuery(string doc_company);
    }
}
