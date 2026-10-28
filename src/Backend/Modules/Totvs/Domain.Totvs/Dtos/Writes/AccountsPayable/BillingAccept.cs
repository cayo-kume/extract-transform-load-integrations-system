namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// aceite da parcela: código de barras e CNPJ do cedente (BillingAcceptModel)
    /// </summary>
    public class BillingAccept
    {
        public string? barCode { get; init; }
        public string? assignorCnpj { get; init; }
    }
}
