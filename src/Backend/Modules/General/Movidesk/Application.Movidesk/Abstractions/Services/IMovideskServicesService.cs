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

        /// <summary>
        /// cria um serviço (POST services?returnAllProperties=true; sem id, a Movidesk gera) e registra a chamada em
        /// general.MovideskRequests. Se a API devolver o serviço inteiro, ele é gravado; senão, chega pelo próximo GetServices.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostService(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Services.Service service);

        /// <summary>
        /// altera um serviço (PATCH services?id={id}): só os campos enviados mudam, e categories substitui a lista inteira.
        /// O serviço é relido e gravado logo depois.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchServiceById(string user_name, string doc_company, int id, Domain.Movidesk.Dtos.Writes.Services.Service service);

        /// <summary>
        /// exclui um serviço do catálogo (DELETE services?id={id}), irreversível. O serviço continua no banco local.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> DeleteServiceById(string user_name, string doc_company, int id);
    }
}
