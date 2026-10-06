namespace Application.Oto.Interfaces.Handlers.Commands
{
    public interface IOtoCommandHandler
    {
        string CreateGetParametersQuery();
        
        string CreateGetOrdersQuery();

        string CreateGetProductsQuery();
        
        string CreateGetClientsQuery();

        string CreateGetClientsCashBackQuery();
        
        string CreateGetCompanysQuery();
        
        string CreateGetSaleRepresentativesQuery();
    }
}
