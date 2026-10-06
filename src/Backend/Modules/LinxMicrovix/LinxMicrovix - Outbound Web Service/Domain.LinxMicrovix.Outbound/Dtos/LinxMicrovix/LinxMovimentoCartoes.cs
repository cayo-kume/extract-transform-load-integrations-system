namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxMovimentoCartoes
    {
        public string? identificador { get; set; }
        public string? portal { get; set; }
        public string? cnpj_emp { get; set; }
        public string? codlojasitef { get; set; }
        public string? data_lancamento { get; set; }
        public string? cupomfiscal { get; set; }
        public string? credito_debito { get; set; }
        public string? id_cartao_bandeira { get; set; }
        public string? descricao_bandeira { get; set; }
        public string? valor { get; set; }
        public string? ordem_cartao { get; set; }
        public string? nsu_host { get; set; }
        public string? nsu_sitef { get; set; }
        public string? cod_autorizacao { get; set; }
        public string? id_antecipacoes_financeiras { get; set; }
        public string? transacao_servico_terceiro { get; set; }
        public string? texto_comprovante { get; set; }
        public string? id_maquineta_pos { get; set; }
        public string? descricao_maquineta { get; set; }
        public string? serie_maquineta { get; set; }
        public string? timestamp { get; set; }
        public string? cartao_prepago { get; set; }

        public LinxMovimentoCartoes()
        {
        }

        public LinxMovimentoCartoes(string? identificador, string? portal, string? cnpj_emp, string? codlojasitef, string? data_lancamento, string? cupomfiscal, string? credito_debito, string? id_cartao_bandeira, string? descricao_bandeira, string? valor, string? ordem_cartao, string? nsu_host, string? nsu_sitef, string? cod_autorizacao, string? id_antecipacoes_financeiras, string? transacao_servico_terceiro, string? texto_comprovante, string? id_maquineta_pos, string? descricao_maquineta, string? serie_maquineta, string? timestamp, string? cartao_prepago)
        {
            this.identificador = identificador;
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.codlojasitef = codlojasitef;
            this.data_lancamento = data_lancamento;
            this.cupomfiscal = cupomfiscal;
            this.credito_debito = credito_debito;
            this.id_cartao_bandeira = id_cartao_bandeira;
            this.descricao_bandeira = descricao_bandeira;
            this.valor = valor;
            this.ordem_cartao = ordem_cartao;
            this.nsu_host = nsu_host;
            this.nsu_sitef = nsu_sitef;
            this.cod_autorizacao = cod_autorizacao;
            this.id_antecipacoes_financeiras = id_antecipacoes_financeiras;
            this.transacao_servico_terceiro = transacao_servico_terceiro;
            this.texto_comprovante = texto_comprovante;
            this.id_maquineta_pos = id_maquineta_pos;
            this.descricao_maquineta = descricao_maquineta;
            this.serie_maquineta = serie_maquineta;
            this.timestamp = timestamp;
            this.cartao_prepago = cartao_prepago;
        }
    }
}
