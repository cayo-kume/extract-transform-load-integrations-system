namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Produtos. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixProductsService
    {
        /// <summary>
        /// o cadastro de produtos de acordo com a loja pesquisada ou dados da estrutura mercadológica
        /// </summary>
        public Task<bool?> LinxProdutos();

        /// <summary>
        /// a associação entre produtos.
        /// </summary>
        public Task<bool?> LinxProdutosAssociados();

        /// <summary>
        /// os campos adicionais preenchidos dos Produtos cadastrados no portal
        /// </summary>
        public Task<bool?> LinxProdutosCamposAdicionais();

        /// <summary>
        /// o cadastro de códigos de barras dos produtos
        /// </summary>
        public Task<bool?> LinxProdutosCodBar();

        /// <summary>
        /// os registros de produtos x fornecedores
        /// </summary>
        public Task<bool?> LinxProdutosFornec();

        /// <summary>
        /// informações dos produtos (características).
        /// </summary>
        public Task<bool?> LinxProdutosInformacoes();

        /// <summary>
        /// as URLs das imagens dos produtos.
        /// </summary>
        public Task<bool?> LinxProdutosImagensURL();

        /// <summary>
        /// o saldo do lotes de produtos.
        /// </summary>
        public Task<bool?> LinxProdutosLotes();

        /// <summary>
        /// os dados dos lotes dos produtos.
        /// </summary>
        public Task<bool?> LinxLotesProdutos();

        /// <summary>
        /// o saldo atual dos seriais cadastrados no portal.
        /// </summary>
        public Task<bool?> LinxProdutosSerial();

        /// <summary>
        /// sob demanda: busca LinxProdutos por cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosByCodProduto(string user_name, string doc_company, string cod_produto);

        /// <summary>
        /// sob demanda: busca LinxProdutosCamposAdicionais por cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosCamposAdicionaisByCodProduto(string user_name, string doc_company, string cnpj_emp, string cod_produto);

        /// <summary>
        /// sob demanda: busca LinxProdutosCodBar por cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosCodBarByCodProduto(string user_name, string doc_company, string cod_produto);
    }
}
