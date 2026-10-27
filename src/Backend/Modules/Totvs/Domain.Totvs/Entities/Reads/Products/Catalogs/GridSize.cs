using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// tamanho da grade, na posição em que a API o devolve (a lista grid é de textos, sem chave, e pode repetir um tamanho), tabela totvs.TotvsGridsSizes
    /// </summary>
    public class GridSize
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int gridCode { get; private set; }
        public int sequence { get; private set; }
        public string sizeName { get; private set; } = String.Empty;

        public GridSize() { }

        public GridSize(string doc_company, DateTime integrated_or_updated_at, int gridCode, int sequence, string? sizeName)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.gridCode = gridCode;

            this.sequence = sequence;
            this.sizeName = ConverterValidatorsExtensions.StringLengthValidation(sizeName, 20);
        }
    }
}
