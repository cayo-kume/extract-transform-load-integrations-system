namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Pagamento. (PaymentInDto)
    /// </summary>
    public class B2COrderPayment
    {
        public B2CPaymentDocumentType? documentType { get; init; }
        // Código do NSU do pagamento em cartão.
        public long? nsu { get; init; }
        // Código do NSU alfanumérico.
        public string? alphanumericNsu { get; init; }
        // Código da autorização do pagamento em cartão.
        public string? authorizationCode { get; init; }
        // Identificador da operadora de cartão de crédito.
        public string? creditCardOperator { get; init; }
        // Identificador da bandeira de cartão de crédito.
        public string? creditCardBrand { get; init; }
        // Código do portador.
        public int? assignorCode { get; init; }
        // Quantidade de parcelas do pagamento.
        public int? installment { get; init; }
        // Valor do pagamento.
        public decimal? paymentValue { get; init; }
        // Número do documento.
        public long? documentNumber { get; init; }
        // Código do operador financeiro.
        public int? financialOperatorCode { get; init; }
        // CPF ou CNPJ do operador financeiro.
        public string? financialOperatorCnpj { get; init; }
        // Código da conta corrente.
        public int? currentAccountCode { get; init; }
        public B2CPaymentType? paymentType { get; init; }
        // Empresa do financeiro.
        public int? paymentBranch { get; init; }
        // Cnpj da Empresa do financeiro.
        public string? paymentBranchCnpj { get; init; }
        // Nosso número referente ao financeiro.
        public long? ourNumber { get; init; }
    }
}
