namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// tipo de documento da duplicata gerada pelo agrupamento: vai como número (T3)
    /// </summary>
    public enum GroupDocumentType
    {
        Duplicate = 1,   // duplicata
        FreightBill = 18   // conhecimento de frete
    }
}
