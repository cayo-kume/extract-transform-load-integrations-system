namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// estágios aceitos pela troca de estágio, com os números da descrição do swagger: o enum vai como número (D6/T3 do
    /// docs/PLANO-TOTVS-METODOS-FALTANTES.md). Transições válidas na Totvs: 1 → 5 ou 2; 5 → 2 ou 1; 2 → 1 ou 5
    /// </summary>
    public enum ChangeInstallmentStageType
    {
        InvoiceNotConfered = 1,   // título não conferido
        ReleasedForPayment = 2,   // liberado para pagamento
        InvoiceAccept = 5         // título aceito
    }
}
