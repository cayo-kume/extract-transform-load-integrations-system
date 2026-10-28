namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// estágio da parcela na inclusão (campo "Estágio" do FCPFM004), com os números da descrição do swagger: vai como número (T3)
    /// </summary>
    public enum StageType
    {
        InvoiceNotConfered = 1,   // título não conferido
        ReleasedForPayment = 2,   // liberado para pagamento
        AuthorizedCheck = 3,   // cheque autorizado
        CheckIssued = 4,   // cheque emitido
        InvoiceAccept = 5,   // título aceito
        Endossado = 10,   // endossado
        PaymentInBank = 20,   // pagamento em banco
        Reserved = 30,   // reservado
        PaymentAutomatic = 40,   // pagamento automático
        ShippingInvoiceSchedulingSupplier = 41,   // envio título agendamento fornecedor
        InvoiceSchedulingSupplier = 42,   // título em agendamento fornecedor
        PaymentImpost = 43,   // pagamento tributo
        WithRestriction = 70,   // com restrição
        WithPendingRestriction = 71,   // com restrição pendência
        Finished = 90   // liquidado
    }
}
