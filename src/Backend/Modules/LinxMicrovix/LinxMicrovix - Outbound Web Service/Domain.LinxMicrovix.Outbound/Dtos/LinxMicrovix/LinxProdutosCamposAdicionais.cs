

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosCamposAdicionais
    {
        public string? portal { get; private set; }
        public string? cod_produto { get; private set; }
        public string? campo { get; private set; }
        public string? valor { get; private set; }
        public string? timestamp { get; private set; }

        public LinxProdutosCamposAdicionais() { }

        public LinxProdutosCamposAdicionais(
            string? portal,
            string? cod_produto,
            string? campo,
            string? valor,
            string? timestamp
        )
        {
            this.cod_produto = cod_produto;
            this.campo = campo;
            this.valor = valor;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}