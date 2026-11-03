namespace Application.Shared.Abstractions.Services
{
    /// <summary>
    /// chamado pelo LoggerService antes de abrir o log de uma execução (docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 4.1). Nos hosts
    /// de agenda não faz nada: quem agenda já segura a trava. No WebApi, uma rota que roda um job da agenda pede a mesma trava dos
    /// agendadores e, com ela ocupada, recebe JobAlreadyRunningException (409), sem abrir log
    /// </summary>
    public interface IJobExecutionGuard
    {
        public void BeforeLogStart(Enum job, string company);
    }
}
