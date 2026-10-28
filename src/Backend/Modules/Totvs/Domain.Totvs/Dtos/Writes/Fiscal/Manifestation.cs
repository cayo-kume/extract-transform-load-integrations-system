namespace Domain.Totvs.Dtos.Writes.Fiscal
{
    /// <summary>
    /// corpo do POST fiscal/v2/invoices/manifestations (UpsertManifestationCommand): inclui ou altera documentos no painel de manifestação do
    /// destinatário (FISFP153). O corpo não traz o tipo do evento; o efeito fiscal (evento na SEFAZ) é a confirmar com a área fiscal (F1)
    /// </summary>
    public class Manifestation
    {
        public List<ManifestationItem>? items { get; init; }
    }
}
