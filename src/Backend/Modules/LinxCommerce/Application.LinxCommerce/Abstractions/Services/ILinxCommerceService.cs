using Domain.LinxCommerce.Entities.Parameters;

namespace Application.LinxCommerce.Abstractions.Services
{
    /// <summary>
    /// o que é comum aos services do módulo (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.1): horário do lote, clientes, a busca
    /// paginada e o Get de um registro, com a autenticação Basic do cliente
    /// </summary>
    public interface ILinxCommerceService
    {
        public DateTime GetBatchTime();

        /// <summary>
        /// clientes de linxcommerce.LinxCommerceParameters com CNPJ, endereço e credenciais preenchidos (cache LinxCommerce:Parameters)
        /// </summary>
        public Task<IEnumerable<Company>> GetCompanys();

        /// <summary>
        /// filtro do Where das buscas pela data de alteração, no horário de Brasília (o formato do código antigo)
        /// </summary>
        public string ModifiedDateWindow(DateTime since, DateTime until);

        /// <summary>
        /// lê todas as páginas de uma busca (POST v1/{route} com Page, Where e OrderBy) e devolve cada item com o seu JSON.
        /// Devolve null se alguma página falhou (a mensagem vai para o log): leitura incompleta não é gravada
        /// </summary>
        public Task<List<(T dto, string json)>?> Search<T>(Company company, string route, string where, string orderBy);

        /// <summary>
        /// lê um registro (POST v1/{route} com o id no corpo) e devolve o DTO e o JSON sem os campos sensíveis, ou null se a
        /// chamada falhou (a mensagem vai para o log)
        /// </summary>
        public Task<(T dto, string json)?> Get<T>(Company company, string route, object body, string key) where T : class;
    }
}
