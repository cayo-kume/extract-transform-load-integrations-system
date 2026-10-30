using Domain.LinxMicrovix.Outbound.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.Enums;

namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo; o envelope XML, a chamada à API, a repetição pelo timestamp e a
    /// leitura da resposta ficam no próprio método de cada service (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1)
    /// </summary>
    public interface ILinxMicrovixService
    {
        /// <summary>
        /// horário do lote: todos os registros de uma gravação levam o mesmo integrated_or_updated_at
        /// </summary>
        public DateTime GetBatchTime();

        /// <summary>
        /// clientes de linxmicrovix.LinxMicrovixParameters, em cache
        /// </summary>
        public Task<IEnumerable<Company>> GetCompanys();

        /// <summary>
        /// um cliente de linxmicrovix.LinxMicrovixParameters pelo CNPJ (rotas sob demanda); null quando não está cadastrado
        /// </summary>
        public Task<Company?> GetCompany(string doc_company);

        /// <summary>
        /// registra uma chamada sob demanda à API do Microvix em linxmicrovix.LinxMicrovixRequests. O corpo é o XML enviado,
        /// já sem a chave
        /// </summary>
        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null);

        /// <summary>
        /// CNPJs das lojas do cliente na API (LinxGrupoLojas no ERP, B2CConsultaEmpresas no B2C), só as que têm CNPJ.
        /// Sem cache: uma loja nova vale já na próxima execução
        /// </summary>
        public Task<List<string>> GetStores(Company company, EnumApi api);

        /// <summary>
        /// maior timestamp gravado do método para o cliente e a loja (cnpj_emp nulo: método de portal); 0 com o banco vazio
        /// </summary>
        public Task<long> GetLastTimestamp(Company company, EnumJob method, string? cnpj_emp);

        /// <summary>
        /// maior timestamp gravado do método para o cliente, a loja e um valor do parâmetro que o método exige (ex.: id_tabela
        /// no LinxProdutosTabelasPrecos), para que a leitura de um valor não pule registros de outro
        /// </summary>
        public Task<long> GetLastTimestamp(Company company, EnumJob method, string? cnpj_emp, string filterColumn, string filterValue);

        /// <summary>
        /// valores distintos de uma coluna de outro método já gravado (ex.: os id_tabela de LinxProdutosTabelas), para os
        /// métodos que exigem um deles por chamada. Com changedSince, só os valores gravados depois da última gravação do
        /// método que os consome (leitura incremental dos métodos que exigem um registro por chamada, como cod_pedido)
        /// </summary>
        public Task<List<string>> GetSourceValues(Company company, EnumJob source, string sourceColumn, string? cnpj_emp, EnumJob? changedSince);
    }
}
