namespace Application.Shared.Exceptions
{
    /// <summary>
    /// a rota pediu para rodar um job da agenda que já está rodando (agendado ou por outro disparo): o WebApi devolve 409, sem log
    /// em auditing
    /// </summary>
    public class JobAlreadyRunningException : Exception
    {
        public JobAlreadyRunningException(int idJob, string key)
            : base($"O job {idJob} já está rodando (agendado ou por outro disparo, trava {key}). Tente quando terminar.") =>
            (IdJob, Key) = (idJob, key);

        public int IdJob { get; }

        public string Key { get; }
    }
}
