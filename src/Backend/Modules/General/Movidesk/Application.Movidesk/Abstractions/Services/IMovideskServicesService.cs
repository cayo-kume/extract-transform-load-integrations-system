namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskServicesService
    {
        /// <summary>
        /// busca um serviço por id, grava nas tabelas de serviços e registra a chamada em general.MovideskRequests.
        /// true: gravado | false: o serviço não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
        /// </summary>
        public Task<bool?> GetServiceById(string user_name, string doc_company, int id);

        public Task<bool?> GetServices();
    }
}
