namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Tipo de cobrança. (ChargeType): vai como número (T3)
    /// </summary>
    public enum B2COrderChargeType
    {
        Carteira = 0,   // Carteira
        CobrancaNaLojaEmpresa = 16,   // Cobrança na loja/empresa
        AguardandoRecebimento = 17,   // Aguardando recebimento
        BoletoBancarioDireto = 18,   // Boleto bancário direto
        Exportacao = 28   // Exportação
    }
}
