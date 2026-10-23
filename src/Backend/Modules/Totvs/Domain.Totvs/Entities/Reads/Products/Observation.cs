namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// observação da referência, tabela totvs.TotvsSkusObservations (sem chave na API: a posição é a chave)
    /// </summary>
    public class Observation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public string? observation { get; private set; }   // nvarchar(max)

        public Observation() { }

        public Observation(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, int sequence, Dtos.Reads.Products.Observation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.sequence = sequence;
            observation = dto.observation;
        }
    }
}
