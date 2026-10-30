using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// cor do cadastro mestre (POST product/v2/colors/search, ColorDataResponseModel), tabela totvs.TotvsColors e filhas. Sem maxChangeFilterDate na resposta: json_hash (SHA-256 do JSON da cor) diz se ela mudou (T9 do src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class Color
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string colorCode { get; private set; } = String.Empty;
        public string colorName { get; private set; } = String.Empty;
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<ColorAdditionalInformation> additionalInformation { get; private set; } = [];

        public Color() { }

        public Color(string doc_company, DateTime integrated_or_updated_at, string json_hash, Dtos.Reads.Products.Catalogs.Color dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            colorCode = ConverterValidatorsExtensions.StringLengthValidation(dto.colorCode, 10);
            colorName = ConverterValidatorsExtensions.StringLengthValidation(dto.colorName, 40);

            // repetidos violariam a PK
            additionalInformation.AddRange((dto.additionalColorInformation ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode) != 0).Select(x => new ColorAdditionalInformation(doc_company, integrated_or_updated_at, colorCode, x)).GroupBy(x => x.branchCode).Select(g => g.Last()));
        }
    }
}
