namespace Domain.Shared.Enums
{
    /// <summary>
    /// nível de uma mensagem de auditing.Messages (coluna Level, CK_Messages_Level), src/Backend/Shared/.docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md.
    /// A pergunta que separa aviso de erro: o dado ficou para trás por uma falha (erro) ou por uma condição conhecida, que se
    /// resolve com cadastro ou rodando outro job antes (aviso)?
    /// </summary>
    public enum MessageLevel : byte
    {
        /// <summary>
        /// a execução seguiu o caminho esperado: progresso, totais e o resumo final
        /// </summary>
        Information = 1,

        /// <summary>
        /// parte do que devia ser processado não foi, por um motivo do nosso lado (cadastro, ordem dos jobs, descarte de propósito)
        /// </summary>
        Warning = 2,

        /// <summary>
        /// a execução falhou, no todo ou em parte: exceção, ou a fonte externa respondeu com erro ou com algo que não dá para ler
        /// </summary>
        Error = 3
    }
}
