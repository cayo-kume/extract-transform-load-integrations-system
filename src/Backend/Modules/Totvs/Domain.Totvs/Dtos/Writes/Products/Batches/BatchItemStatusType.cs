namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// situação do item de lote (campo "Situação" do PRDFM059): vai como número (T3)
    /// </summary>
    public enum BatchItemStatusType
    {
        Approved = 1,   // aprovado
        ApprovesRestriction = 2,   // aprovado com restrição
        Blocked = 4,   // bloqueado
        WaitingForRelease = 10   // aguardando liberação
    }
}
