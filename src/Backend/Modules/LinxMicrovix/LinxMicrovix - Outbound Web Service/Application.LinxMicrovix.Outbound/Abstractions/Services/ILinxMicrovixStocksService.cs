namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Estoque e balanços. Os nomes dos métodos são os da API (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixStocksService
    {
        /// <summary>
        /// informações dos produtos de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxProdutosDetalhes();

        /// <summary>
        /// os dados adicionais de produtos de forma simplificada
        /// </summary>
        public Task<bool?> LinxProdutosDetalhesSimplificado();

        /// <summary>
        /// os saldos dos produtos por depósito.
        /// </summary>
        public Task<bool?> LinxProdutosDetalhesDepositos();

        /// <summary>
        /// os depósitos cadastrados no ERP
        /// </summary>
        public Task<bool?> LinxProdutosDepositos();

        /// <summary>
        /// informações do saldo do produto na data pesquisada
        /// </summary>
        public Task<bool?> LinxProdutosInventario();

        /// <summary>
        /// a listagem de balanços.
        /// </summary>
        public Task<bool?> LinxListagemBalanco();

        /// <summary>
        /// os balanços.
        /// </summary>
        public Task<bool?> LinxBalancos();

        /// <summary>
        /// os dados dos arquivos de balanços.
        /// </summary>
        public Task<bool?> LinxBalancosArquivos();

        /// <summary>
        /// as seções dos balanços.
        /// </summary>
        public Task<bool?> LinxBalancosSecoes();

        /// <summary>
        /// sob demanda: busca LinxProdutosDetalhes por cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosDetalhesByCodProduto(string user_name, string doc_company, string cnpj_emp, string cod_produto);

        /// <summary>
        /// sob demanda: busca LinxProdutosDepositos por cod_deposito, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosDepositosByCodDeposito(string user_name, string doc_company, string cod_deposito);

        /// <summary>
        /// sob demanda: busca LinxProdutosInventario por cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosInventarioByCodProduto(string user_name, string doc_company, string cnpj_emp, string cod_produto);
    }
}
