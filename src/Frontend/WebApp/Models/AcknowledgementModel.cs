namespace WebApp.Models
{
    /// <summary>
    /// Reconhecimento de uma falha (Erro ou Atrasado) por quem opera o painel: o job sai de "Falhas Ativas" até a próxima
    /// execução ou até expirar, e a nota fica para o próximo turno. A saúde do job não muda: é uma camada por cima dela.
    /// Na fase 5, vira a tabela monitor.JobAcknowledgement (docs/PLANO-WEBAPP.md).
    /// </summary>
    public class AcknowledgementModel
    {
        /// <summary>
        /// Execução reconhecida (a última do job no momento); nula para job atrasado que nunca rodou.
        /// </summary>
        public Guid? Execution { get; set; }

        public string? Note { get; set; }
        public string? User { get; set; }
        public DateTime At { get; set; }

        /// <summary>
        /// Mesmo sem execução nova, o reconhecimento expira aqui, para um job que nunca mais rodou voltar a aparecer.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
