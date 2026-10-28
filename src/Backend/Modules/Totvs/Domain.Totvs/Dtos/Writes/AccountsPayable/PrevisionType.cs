namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// tipo de previsão da duplicata (campo "Tipo previsão" do FCPFM004): vai como número (T3). Com Real, a arrivalDate vira a data
    /// da liquidação de inclusão
    /// </summary>
    public enum PrevisionType
    {
        Forecast = 1,   // previsão
        Real = 2,   // real
        Consignment = 3   // consignação
    }
}
