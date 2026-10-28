namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// tipo de retenção do imposto da duplicata: vai como número (T3)
    /// </summary>
    public enum RetentionType
    {
        Direct = 1,   // direto
        RoffValue = 2,   // valor de teto
        MinimumTaxAmountPerMonth = 3,   // valor mínimo de imposto por mês
        MaximumTaxAmountPerDuplicate = 4,   // valor mínimo de imposto por duplicata
        RoffValuePerDuplicate = 5,   // valor de teto por duplicata
        RoofDayValue = 6   // valor de teto por dia
    }
}
