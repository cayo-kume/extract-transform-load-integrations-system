namespace WebApp.Models
{
    /// <summary>
    /// Resumo de uma aplicação no período, para os cards.
    /// </summary>
    public class AppSummaryModel
    {
        public int IdParent { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Imagem da aplicação para a empresa logada; nula quando não há imagem (o card mostra as iniciais).
        /// </summary>
        public string? ImageUrl { get; set; }

        public int Integrations { get; set; }
        public int Procedures { get; set; }
        public int ActiveJobs { get; set; }

        public int Executions { get; set; }

        /// <summary>
        /// Execuções com ao menos uma mensagem de erro (nível 3).
        /// </summary>
        public int FailedExecutions { get; set; }

        public int Records { get; set; }
        public int Warnings { get; set; }
        public int Errors { get; set; }

        /// <summary>
        /// Início da execução com erro mais recente no período.
        /// </summary>
        public DateTime? LastErrorAt { get; set; }

        /// <summary>
        /// Jobs ativos cuja última execução tem erro (saúde Erro), no momento da consulta.
        /// </summary>
        public int FailingJobs { get; set; }

        public bool IsActive => ActiveJobs > 0;

        /// <summary>
        /// Falso: módulo do catálogo que a empresa ainda não contratou (sem jobs). A tela Aplicações mostra para contratar; a
        /// Visão Geral, só no Adicionar aplicação.
        /// </summary>
        public bool IsContracted { get; set; } = true;
    }
}
