namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoCartoes: informações dos cartões de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoCartoes
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? codlojasitef { get; init; }
        public string? data_lancamento { get; init; }
        public string? identificador { get; init; }
        public string? cupomfiscal { get; init; }
        public string? credito_debito { get; init; }
        public string? id_cartao_bandeira { get; init; }
        public string? descricao_bandeira { get; init; }
        public string? valor { get; init; }
        public string? ordem_cartao { get; init; }
        public string? nsu_host { get; init; }
        public string? nsu_sitef { get; init; }
        public string? cod_autorizacao { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? transacao_servico_terceiro { get; init; }
        public string? texto_comprovante { get; init; }
        public string? id_maquineta_pos { get; init; }
        public string? descricao_maquineta { get; init; }
        public string? serie_maquineta { get; init; }
        public string? timestamp { get; init; }
        public string? cartao_prepago { get; init; }
        public string? transacao_cancelada_smartpos { get; init; }
        public string? usuario_cancelamento_transacao_smartpos { get; init; }
        public string? data_hora_cancelamento_transacao_smartpos { get; init; }
    }
}
