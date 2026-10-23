using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// tamanho da grade da referência, tabela totvs.TotvsSkusGrids (lista simples na API: a posição é a chave)
    /// </summary>
    public class Grid
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public string grid { get; private set; } = String.Empty;

        public Grid() { }

        public Grid(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, int sequence, string? grid)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.sequence = sequence;
            this.grid = ConverterValidatorsExtensions.StringLengthValidation(grid, 60);
        }
    }
}
