namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras de lojas, usuários e catálogo do Web Service de Saída Padrão (ERP). Os nomes dos métodos são os da API
    /// (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixStoresService
    {
        /// <summary>
        /// lojas do grupo de cada cliente; é o primeiro método: as lojas gravadas aqui são as consultadas pelos outros
        /// </summary>
        public Task<bool?> LinxGrupoLojas();

        /// <summary>
        /// cadastro de cada loja do grupo, pelo CNPJ
        /// </summary>
        public Task<bool?> LinxLojas();

        /// <summary>
        /// parâmetros do portal para cada loja do grupo, pelo CNPJ
        /// </summary>
        public Task<bool?> LinxLojasParametros();

        /// <summary>
        /// usuários do portal (método de portal: sem cnpjEmp)
        /// </summary>
        public Task<bool?> LinxUsuarios();

        /// <summary>
        /// catálogo de métodos da API com os parâmetros de cada um (sem parâmetro nenhum, nem a chave)
        /// </summary>
        public Task<bool?> LinxMetodos();

        /// <summary>
        /// entrantes por período e loja, dos últimos 2 anos (método de portal: sem cnpjEmp)
        /// </summary>
        public Task<bool?> LinxEntrantes();
    }
}
