using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// referência (POST product/v2/references/search), tabela totvs.TotvsSkus. branchInfoCode é a empresa base da
    /// chamada (matriz da marca): os campos por empresa vêm dela, então entra na chave (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4)
    /// </summary>
    public class Sku
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public string referenceCode { get; private set; } = String.Empty;
        public string lastReferenceCode { get; private set; } = String.Empty;
        public DateTime? insertDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public string? descriptive { get; private set; }   // nvarchar(max)
        public int gridCode { get; private set; }
        public decimal? weight { get; private set; }
        public decimal? height { get; private set; }
        public decimal? width { get; private set; }
        public decimal? length { get; private set; }
        public decimal? packWeight { get; private set; }
        public decimal? packHeight { get; private set; }
        public decimal? packWidth { get; private set; }
        public decimal? packLength { get; private set; }

        [SkipProperty]
        public List<Grid> grids { get; private set; } = [];

        [SkipProperty]
        public List<Observation> observations { get; private set; } = [];

        [SkipProperty]
        public List<Detail> details { get; private set; } = [];

        [SkipProperty]
        public List<Composition> compositions { get; private set; } = [];

        [SkipProperty]
        public List<ReferenceCategory> referenceCategories { get; private set; } = [];

        [SkipProperty]
        public List<IntegrationGroupCode> integrationGroupCodes { get; private set; } = [];

        [SkipProperty]
        public List<Color> colors { get; private set; } = [];

        public Sku() { }

        public Sku(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, Dtos.Reads.Products.Reference dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;

            referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.referenceId);
            referenceCode = ConverterValidatorsExtensions.StringLengthValidation(dto.ReferenceCode, 60);
            lastReferenceCode = ConverterValidatorsExtensions.StringLengthValidation(dto.lastReferenceCode, 60);
            insertDate = NullableConverters.ToNullableDateTime(dto.insertDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 250);
            descriptive = dto.descriptive;
            gridCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.gridCode);
            weight = NullableConverters.ToNullableDecimal(dto.weight);
            height = NullableConverters.ToNullableDecimal(dto.height);
            width = NullableConverters.ToNullableDecimal(dto.width);
            length = NullableConverters.ToNullableDecimal(dto.length);
            packWeight = NullableConverters.ToNullableDecimal(dto.packWeight);
            packHeight = NullableConverters.ToNullableDecimal(dto.packHeight);
            packWidth = NullableConverters.ToNullableDecimal(dto.packWidth);
            packLength = NullableConverters.ToNullableDecimal(dto.packLength);

            // grade, observações, detalhes e composição não têm chave na API: a posição na lista é a chave
            // (typeCode e auxiliaryType dos detalhes se repetem na mesma referência, testado)
            grids.AddRange((dto.grid ?? []).Select((g, i) => new Grid(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, i + 1, g)));
            observations.AddRange((dto.observations ?? []).Select((o, i) => new Observation(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, i + 1, o)));
            details.AddRange((dto.details ?? []).Select((d, i) => new Detail(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, i + 1, d)));
            compositions.AddRange((dto.composition ?? []).Select((c, i) => new Composition(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, i + 1, c)));

            // repetidos violariam a PK
            referenceCategories.AddRange((dto.referenceCategories ?? []).Select(c => new ReferenceCategory(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, c)).GroupBy(c => c.code).Select(g => g.Last()));
            integrationGroupCodes.AddRange((dto.integrationGroupCodes ?? []).Where(c => !String.IsNullOrWhiteSpace(c.code)).Select(c => new IntegrationGroupCode(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, c)).GroupBy(c => c.code).Select(g => g.Last()));
            colors.AddRange((dto.colors ?? []).Select(c => new Color(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, c)).GroupBy(c => c.colorCode).Select(g => g.Last()));
        }
    }
}
