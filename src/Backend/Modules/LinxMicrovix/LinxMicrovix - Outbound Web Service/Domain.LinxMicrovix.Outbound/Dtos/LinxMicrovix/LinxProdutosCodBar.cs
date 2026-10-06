

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosCodBar
    {
        public string? portal { get; private set; }
        public string? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? timestamp { get; private set; }

        public LinxProdutosCodBar() { }

        public LinxProdutosCodBar(
            string? portal,
            string? cod_produto,
            string? cod_barra,
            string? timestamp
        )
        {
            this.cod_produto = cod_produto;
            this.cod_barra = cod_barra;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}