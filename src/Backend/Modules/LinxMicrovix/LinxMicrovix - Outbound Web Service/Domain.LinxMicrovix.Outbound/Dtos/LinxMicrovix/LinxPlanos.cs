

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxPlanos
    {
        public string? portal { get; private set; }
        public string? plano { get; private set; }
        public string? desc_plano { get; private set; }
        public string? qtde_parcelas { get; private set; }
        public string? prazo_entre_parcelas { get; private set; }
        public string? tipo_plano { get; private set; }
        public string? indice_plano { get; private set; }
        public string? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public string? conta_central { get; private set; }
        public string? tipo_transacao { get; private set; }
        public string? taxa_financeira { get; private set; }
        public string? dt_upd { get; private set; }
        public string? desativado { get; private set; }
        public string? usa_tef { get; private set; }
        public string? timestamp { get; private set; }

        public LinxPlanos() { }

        public LinxPlanos(
            string? portal,
            string? plano,
            string? desc_plano,
            string? qtde_parcelas,
            string? prazo_entre_parcelas,
            string? tipo_plano,
            string? indice_plano,
            string? cod_forma_pgto,
            string? forma_pgto,
            string? conta_central,
            string? tipo_transacao,
            string? taxa_financeira,
            string? dt_upd,
            string? desativado,
            string? usa_tef,
            string? timestamp
        )
        {
            this.portal = portal;
            this.plano = plano;
            this.desc_plano = desc_plano;
            this.qtde_parcelas = qtde_parcelas;
            this.prazo_entre_parcelas = prazo_entre_parcelas;
            this.tipo_plano = tipo_plano;
            this.indice_plano = indice_plano;
            this.cod_forma_pgto = cod_forma_pgto;
            this.forma_pgto = forma_pgto;
            this.conta_central = conta_central;
            this.tipo_transacao = tipo_transacao;
            this.taxa_financeira = taxa_financeira;
            this.dt_upd = dt_upd;
            this.desativado = desativado;
            this.usa_tef = usa_tef;
            this.timestamp = timestamp;
        }
    }
}