namespace Application.Shared.Abstractions.Services
{
    /// <summary>
    /// trava por job, para duas execuções do mesmo job (ou do mesmo grupo) nunca rodarem juntas, nem em servidores diferentes
    /// (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.7). Não depende do agendador: o host Hangfire usa pelo filtro de
    /// concorrência, e os outros hosts podem usar do mesmo jeito
    /// </summary>
    public interface IJobLock
    {
        /// <summary>
        /// tenta pegar a trava da chave sem esperar. Devolve null quando outra execução com a mesma chave está rodando; com a trava,
        /// devolve um objeto cujo Dispose solta a trava
        /// </summary>
        public IDisposable? TryAcquire(string key);
    }
}
