using Domain.Core.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.LinxMicrovix
{
    public class LinxReshopSaldo
    {
        [ColumnName("rede")]
        public string Rede { get; set; } = string.Empty;

        [ColumnName("loja")]
        public string Loja { get; set; } = string.Empty;

        [ColumnName("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [ColumnName("cliente")]
        public string Cliente { get; set; } = string.Empty;

        [ColumnName("email")]
        public string Email { get; set; } = string.Empty;

        [ColumnName("telefone")]
        public string Telefone { get; set; } = string.Empty;

        [ColumnName("pontos")]
        public long Pontos { get; set; }

        [ColumnName("saldo")]
        public decimal Saldo { get; set; }

        [ColumnName("ticket")]
        public long Ticket { get; set; }

        [ColumnName("data")]
        public DateTime Data { get; set; }

        [ColumnName("lastupdateon")]
        public DateTime LastUpdateOn { get; set; }

        [ColumnName("expiracao_pontos")]
        public long ExpiracaoPontos { get; set; }

        [ColumnName("expiracao_saldo")]
        public decimal ExpiracaoSaldo { get; set; }
    }
}
