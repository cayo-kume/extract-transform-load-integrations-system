

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosDepositos
    {
        public string? portal { get; private set; }
        public string? cod_deposito { get; private set; }
        public string? nome_deposito { get; private set; }
        public string? disponivel { get; private set; }
        public string? disponivel_transferencia { get; private set; }
        public string? timestamp { get; private set; }

        public LinxProdutosDepositos() { }

        public LinxProdutosDepositos(
            string? portal,
            string? cod_deposito,
            string? nome_deposito,
            string? disponivel,
            string? disponivel_transferencia,
            string? timestamp
        )
        {
            this.portal = portal;
            this.cod_deposito = cod_deposito;
            this.nome_deposito = nome_deposito;
            this.disponivel = disponivel;
            this.disponivel_transferencia = disponivel_transferencia;
            this.timestamp = timestamp;
        }
    }
}