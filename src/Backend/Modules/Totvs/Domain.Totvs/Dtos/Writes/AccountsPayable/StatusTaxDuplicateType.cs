namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// situação do imposto da duplicata: vai como número (T3)
    /// </summary>
    public enum StatusTaxDuplicateType
    {
        NotRetained = 0,   // não retido
        DuplicateSelfRetained = 1,   // retido na própria duplicata
        DuplicateAnotherRetained = 2   // retido em outra duplicata
    }
}
