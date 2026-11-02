namespace Application.Shared.Abstractions.Services
{
    /// <summary>
    /// teto da carga inicial (banco vazio) em InitialLoad:MaxMonths (docs/PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md §1.2).
    /// Sem a chave, ou com valor inválido, nada muda: cada leitura começa da data própria dela. Com a chave, nenhuma carga
    /// inicial começa antes de agora − MaxMonths; a leitura incremental (com dado no banco) não passa por aqui
    /// </summary>
    public interface IInitialLoad
    {
        /// <summary>
        /// meses do teto; nulo quando desligado
        /// </summary>
        public int? MaxMonths { get; }

        /// <summary>
        /// a data de início da carga inicial: a própria <paramref name="since"/> ou, se for anterior ao teto, agora − MaxMonths.
        /// O teto só encurta a janela, nunca a alarga
        /// </summary>
        /// <param name="since">início que a leitura usaria sem o teto</param>
        /// <param name="now">o "agora" da leitura, no mesmo fuso de <paramref name="since"/> (o horário de lote do módulo)</param>
        /// <returns></returns>
        public DateTime Since(DateTime since, DateTime now);
    }
}
