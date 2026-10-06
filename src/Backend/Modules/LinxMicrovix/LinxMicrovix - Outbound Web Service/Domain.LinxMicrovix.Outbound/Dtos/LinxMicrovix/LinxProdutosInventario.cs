

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosInventario
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? quantidade { get; private set; }
        public string? cod_deposito { get; private set; }
        public string? empresa { get; private set; }

        public LinxProdutosInventario() { }

        public LinxProdutosInventario(
            string? portal,
            string? cnpj_emp,
            string? cod_produto,
            string? cod_barra,
            string? quantidade,
            string? cod_deposito,
            string? empresa
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.cod_produto = cod_produto;
            this.cod_barra = cod_barra;
            this.quantidade = quantidade;
            this.cod_deposito = cod_deposito;
            this.empresa = empresa;
        }
    }
}