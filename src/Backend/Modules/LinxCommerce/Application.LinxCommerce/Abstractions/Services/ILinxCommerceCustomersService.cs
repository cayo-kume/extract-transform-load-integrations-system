namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceCustomersService
    {
        /// <summary>
        /// sincroniza os clientes alterados desde a última leitura (POST Profile/SearchCustomer pela ModifiedDate e GetPerson ou
        /// GetCompany de cada um)
        /// </summary>
        public Task<bool?> SearchCustomer();
    }
}
