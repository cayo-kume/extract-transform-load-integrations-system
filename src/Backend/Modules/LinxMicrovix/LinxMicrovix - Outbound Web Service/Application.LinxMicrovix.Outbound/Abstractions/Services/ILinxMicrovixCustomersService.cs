namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Clientes e fornecedores. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixCustomersService
    {
        /// <summary>
        /// os Clientes/Fornecedores cadastrados na loja e data pesquisada
        /// </summary>
        public Task<bool?> LinxClientesFornec();

        /// <summary>
        /// os campos adicionais preenchidos dos Clientes/Fornecedores cadastrados no portal
        /// </summary>
        public Task<bool?> LinxClientesFornecCamposAdicionais();

        /// <summary>
        /// todas as classes vinculadas a um cliente/fornecedor
        /// </summary>
        public Task<bool?> LinxClientesFornecClasses();

        /// <summary>
        /// os campos referentes ao contato do cadastro dos Clientes/Fornecedores cadastrados no portal
        /// </summary>
        public Task<bool?> LinxClientesFornecContatos();

        /// <summary>
        /// os parentescos cadastrados
        /// </summary>
        public Task<bool?> LinxClientesFornecContatosParentesco();

        /// <summary>
        /// os dados de crédito avulso dos clientes e fornecedores.
        /// </summary>
        public Task<bool?> LinxClientesFornecCreditoAvulso();

        /// <summary>
        /// o código do vendedor associado ao cadastro do cliente, conforme a tabela clientes_fornec_vendedores.
        /// </summary>
        public Task<bool?> LinxClientesFornecVendedores();

        /// <summary>
        /// endereço de entrega do cliente.
        /// </summary>
        public Task<bool?> LinxClientesEnderecosEntrega();

        /// <summary>
        /// os registros da tabela clientes_rede no portal centralizador.
        /// </summary>
        public Task<bool?> LinxClientesRede();

        /// <summary>
        /// os ramos de atividades cadastrados.
        /// </summary>
        public Task<bool?> LinxRamosAtividade();

        /// <summary>
        /// sob demanda: busca LinxClientesFornec por doc_cliente, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxClientesFornecByDocCliente(string user_name, string doc_company, string doc_cliente);
    }
}
