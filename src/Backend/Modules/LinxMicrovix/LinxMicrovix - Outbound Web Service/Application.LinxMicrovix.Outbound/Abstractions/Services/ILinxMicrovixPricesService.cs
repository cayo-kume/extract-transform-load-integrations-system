namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Preços, promoções e cupons. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixPricesService
    {
        /// <summary>
        /// o cadastro de Tabelas de Preços
        /// </summary>
        public Task<bool?> LinxProdutosTabelas();

        /// <summary>
        /// a relação dos produtos por tabela de preço
        /// </summary>
        public Task<bool?> LinxProdutosTabelasPrecos();

        /// <summary>
        /// informações das promoções dos produtos de acordo com a loja pesquisada
        /// </summary>
        public Task<bool?> LinxProdutosPromocoes();

        /// <summary>
        /// as Ações Promocionais cadastradas no Microvix
        /// </summary>
        public Task<bool?> LinxAcoesPromocionais();

        /// <summary>
        /// os itens da combinação de produtos de ações promocionais.
        /// </summary>
        public Task<bool?> LinxAcoesPromocionaisCombinacaoProdutosItens();

        /// <summary>
        /// os produtos de cortesia de ações promocionais.
        /// </summary>
        public Task<bool?> LinxAcoesPromocionaisProdutosCortesia();

        /// <summary>
        /// as Ações Promocionais realizadas nas movimentações
        /// </summary>
        public Task<bool?> LinxMovimentoAcoesPromocionais();

        /// <summary>
        /// os registros do cadastro de cupons de desconto.
        /// </summary>
        public Task<bool?> LinxCupomDesconto();

        /// <summary>
        /// o id do cupom de desconto, o valor do cupom e identificador das vendas que utilizaram cupons de desconto.
        /// </summary>
        public Task<bool?> LinxCupomDescontoVendas();

        /// <summary>
        /// os motivos de desconto cadastrados no portal
        /// </summary>
        public Task<bool?> LinxMotivoDesconto();

        /// <summary>
        /// sob demanda: busca LinxProdutosTabelas por id_tabela, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosTabelasByIdTabela(string user_name, string doc_company, string cnpj_emp, string id_tabela);

        /// <summary>
        /// sob demanda: busca LinxProdutosTabelasPrecos por id_tabela e cod_produto, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxProdutosTabelasPrecosByIdTabelaAndCodProduto(string user_name, string doc_company, string cnpj_emp, string id_tabela, string cod_produto);
    }
}
