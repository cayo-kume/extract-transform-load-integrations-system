namespace Application.LinxMicrovix.Outbound.Abstractions.Services
{
    /// <summary>
    /// leituras do Web Service de Saída Padrão (ERP): Vendedores e funcionários. Os nomes dos métodos são os da API (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §2.3)
    /// </summary>
    public interface ILinxMicrovixSellersService
    {
        /// <summary>
        /// os Vendedores cadastrados na loja
        /// </summary>
        public Task<bool?> LinxVendedores();

        /// <summary>
        /// as férias cadatradas para os vendedores.
        /// </summary>
        public Task<bool?> LinxVendedoresFerias();

        /// <summary>
        /// as Metas de Vendedores das lojas
        /// </summary>
        public Task<bool?> LinxMetasVendedores();

        /// <summary>
        /// as Metas de Vendedores das lojas por dia
        /// </summary>
        public Task<bool?> LinxMetasVendedoresDia();

        /// <summary>
        /// as comissões do vendedores
        /// </summary>
        public Task<bool?> LinxComissoesVendedores();

        /// <summary>
        /// os registros gerados na rotina de Lista da Vez
        /// </summary>
        public Task<bool?> LinxListaDaVez();

        /// <summary>
        /// o cadastro da funções de funcionários.
        /// </summary>
        public Task<bool?> LinxFuncionariosFuncoes();

        /// <summary>
        /// os registros da tabela tipo_registro_profissional.
        /// </summary>
        public Task<bool?> LinxRegistroProfissional();

        /// <summary>
        /// sob demanda: busca LinxVendedores por cod_vendedor, grava e registra a chamada em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        public Task<bool?> LinxVendedoresByCodVendedor(string user_name, string doc_company, string cnpj_emp, string cod_vendedor);
    }
}
