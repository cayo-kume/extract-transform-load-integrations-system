namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Clientes do B2C. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2CCustomersService
    {
        /// <summary>
        /// B2CConsultaClientes
        /// </summary>
        public Task<bool?> B2CConsultaClientes();

        /// <summary>
        /// B2CConsultaClientesContatos
        /// </summary>
        public Task<bool?> B2CConsultaClientesContatos();

        /// <summary>
        /// B2CConsultaClientesContatosParentesco
        /// </summary>
        public Task<bool?> B2CConsultaClientesContatosParentesco();

        /// <summary>
        /// B2CConsultaClientesEnderecosEntrega
        /// </summary>
        public Task<bool?> B2CConsultaClientesEnderecosEntrega();

        /// <summary>
        /// B2CConsultaClientesEstadoCivil
        /// </summary>
        public Task<bool?> B2CConsultaClientesEstadoCivil();

        /// <summary>
        /// B2CConsultaClientesSaldo
        /// </summary>
        public Task<bool?> B2CConsultaClientesSaldo();

        /// <summary>
        /// B2CConsultaClientesSaldoLinx
        /// </summary>
        public Task<bool?> B2CConsultaClientesSaldoLinx();

        /// <summary>
        /// sob demanda: busca B2CConsultaClientes por doc_cliente, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> B2CConsultaClientesByDocCliente(string user_name, string doc_company, string doc_cliente);
    }
}
