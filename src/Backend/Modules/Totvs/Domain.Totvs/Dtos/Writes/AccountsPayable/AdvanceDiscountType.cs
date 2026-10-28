namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// tipo do desconto pontual e dos descontos por adiantamento (campos "Tipo pontual", "Tipo desconto 1" e "Tipo desconto 2" do
    /// FCPFM004): vai como número (T3). O 7 está no swagger sem descrição
    /// </summary>
    public enum AdvanceDiscountType
    {
        None = 0,   // nenhum
        FixedValueDiscountExpiredDate = 1,   // valor fixo de desconto até o vencimento
        FixedPercentageDiscountExpiredDate = 2,   // percentual fixo de desconto até o vencimento
        ValueDiscountAntecipationBusyDay = 3,   // valor de desconto por antecipação, dia corrido
        ValueDiscountAntecipationBusinessDay = 4,   // valor de desconto por antecipação, dia útil
        PercentageOverValueNominalBusyDay = 5,   // percentual sobre o valor nominal, dia corrido
        PercentageOverValueNominalBusinessDay = 6,   // percentual sobre o valor nominal, dia útil
        DiscountCancellation = 7   // cancelamento do desconto
    }
}
