namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceCustomersService
    {
        /// <summary>
        /// sincroniza os clientes alterados desde a última leitura (POST Profile/SearchCustomer pela ModifiedDate e GetPerson ou
        /// GetCompany de cada um)
        /// </summary>
        public Task<bool?> SearchCustomer();

        /// <summary>
        /// busca um cliente pelo CustomerID (POST Profile/SearchCustomer, para saber o tipo, e GetPerson ou GetCompany) e grava;
        /// as chamadas ficam em linxcommerce.LinxCommerceRequests. true gravado, false não existe no Linx Commerce, null empresa
        /// não cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetCustomer(string user_name, string doc_company, int customerId);
    }
}
