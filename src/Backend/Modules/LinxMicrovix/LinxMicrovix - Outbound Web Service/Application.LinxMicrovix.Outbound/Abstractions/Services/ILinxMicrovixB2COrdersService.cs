namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do WS B2C: Pedidos e notas do B2C. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixB2COrdersService
    {
        /// <summary>
        /// B2CConsultaPedidos
        /// </summary>
        public Task<bool?> B2CConsultaPedidos();

        /// <summary>
        /// B2CConsultaPedidosItens
        /// </summary>
        public Task<bool?> B2CConsultaPedidosItens();

        /// <summary>
        /// B2CConsultaPedidosPlanos
        /// </summary>
        public Task<bool?> B2CConsultaPedidosPlanos();

        /// <summary>
        /// B2CConsultaPedidosStatus
        /// </summary>
        public Task<bool?> B2CConsultaPedidosStatus();

        /// <summary>
        /// os registro da tabela B2C_vendas
        /// </summary>
        public Task<bool?> B2CConsultaPedidosIdentificador();

        /// <summary>
        /// B2CConsultaPedidosTipos
        /// </summary>
        public Task<bool?> B2CConsultaPedidosTipos();

        /// <summary>
        /// B2CConsultaStatus
        /// </summary>
        public Task<bool?> B2CConsultaStatus();

        /// <summary>
        /// B2CConsultaCodigoRastreio
        /// </summary>
        public Task<bool?> B2CConsultaCodigoRastreio();

        /// <summary>
        /// B2CConsultaNFe
        /// </summary>
        public Task<bool?> B2CConsultaNFe();

        /// <summary>
        /// B2CConsultaNFeSituacao
        /// </summary>
        public Task<bool?> B2CConsultaNFeSituacao();

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidos por id_pedido, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> B2CConsultaPedidosByIdPedido(string user_name, string doc_company, string id_pedido);

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidosItens por id_pedido, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> B2CConsultaPedidosItensByIdPedido(string user_name, string doc_company, string id_pedido);

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidosStatus por id_pedido, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> B2CConsultaPedidosStatusByIdPedido(string user_name, string doc_company, string id_pedido);

        /// <summary>
        /// sob demanda: busca B2CConsultaNFe por id_pedido, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> B2CConsultaNFeByIdPedido(string user_name, string doc_company, string cnpj_emp, string id_pedido);
    }
}
