using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// grade de tamanhos (GET product/v2/grid, GridModel), tabela totvs.TotvsGrids e filhas
    /// </summary>
    public class Grid
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<GridSize> sizes { get; private set; } = [];

        public Grid() { }

        public Grid(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.Grid dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 30);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // a posição é a chave: a lista não tem código e pode repetir um tamanho
            sizes.AddRange((dto.grid ?? []).Select((x, i) => new GridSize(doc_company, integrated_or_updated_at, code, i + 1, x)));
        }
    }
}
