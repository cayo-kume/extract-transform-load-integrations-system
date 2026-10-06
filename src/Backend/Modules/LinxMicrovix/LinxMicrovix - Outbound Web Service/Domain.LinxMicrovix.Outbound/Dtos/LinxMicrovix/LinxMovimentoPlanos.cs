

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxMovimentoPlanos
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? identificador { get; private set; }
        public string? plano { get; private set; }
        public string? desc_plano { get; private set; }
        public string? total { get; private set; }
        public string? qtde_parcelas { get; private set; }
        public string? indice_plano { get; private set; }
        public string? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public string? tipo_transacao { get; private set; }
        public string? taxa_financeira { get; private set; }
        public string? ordem_cartao { get; private set; }
        public string? timestamp { get; private set; }
        public string? empresa { get; private set; }

        public LinxMovimentoPlanos() { }

        public LinxMovimentoPlanos(
            string? portal,
            string? cnpj_emp,
            string? identificador,
            string? plano,
            string? desc_plano,
            string? total,
            string? qtde_parcelas,
            string? indice_plano,
            string? cod_forma_pgto,
            string? forma_pgto,
            string? tipo_transacao,
            string? taxa_financeira,
            string? ordem_cartao,
            string? timestamp,
            string? empresa
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.identificador = identificador;
            this.plano = plano;
            this.desc_plano = desc_plano;
            this.total = total;
            this.qtde_parcelas = qtde_parcelas;
            this.indice_plano = indice_plano;
            this.cod_forma_pgto = cod_forma_pgto;
            this.forma_pgto = forma_pgto;
            this.tipo_transacao = tipo_transacao;
            this.taxa_financeira = taxa_financeira;
            this.ordem_cartao = ordem_cartao;
            this.timestamp = timestamp;
            this.empresa = empresa;
        }
    }
}