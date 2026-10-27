namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Atributos de produto. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixProductAttributesService
    {
        /// <summary>
        /// os setores cadastrados
        /// </summary>
        public Task<bool?> LinxSetores();

        /// <summary>
        /// todos os registros do cadastro auxiliar linha
        /// </summary>
        public Task<bool?> LinxLinhas();

        /// <summary>
        /// as marcas cadastradas.
        /// </summary>
        public Task<bool?> LinxMarcas();

        /// <summary>
        /// as coleções cadastradas.
        /// </summary>
        public Task<bool?> LinxColecoes();

        /// <summary>
        /// os tamanhos cadastrados.
        /// </summary>
        public Task<bool?> LinxTamanhos();

        /// <summary>
        /// as cores cadastradas.
        /// </summary>
        public Task<bool?> LinxCores();

        /// <summary>
        /// as espessuras cadastradas.
        /// </summary>
        public Task<bool?> LinxEspessuras();

        /// <summary>
        /// as unidades cadastradas.
        /// </summary>
        public Task<bool?> LinxUnidades();

        /// <summary>
        /// as classificacoes cadastradas.
        /// </summary>
        public Task<bool?> LinxClassificacoes();

        /// <summary>
        /// o cadastro das Sub Classes dos clientes e fornecedores.
        /// </summary>
        public Task<bool?> LinxSubClasses();

        /// <summary>
        /// sob demanda: busca LinxSetores por id_setor, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxSetoresByIdSetor(string user_name, string doc_company, string id_setor);
    }
}
