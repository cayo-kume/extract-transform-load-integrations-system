using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// produto (SKU) da cor, tabela totvs.TotvsSkusColorsProducts. O código é único no cadastro: a chave é
    /// doc_company + branchInfoCode + code, e as filhas do produto usam essa chave (productCode)
    /// </summary>
    public class Product
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int code { get; private set; }
        public int referenceId { get; private set; }
        public string colorCode { get; private set; } = String.Empty;
        public string sku { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string ncm { get; private set; } = String.Empty;
        public decimal? ipi { get; private set; }
        public string cst { get; private set; } = String.Empty;
        public string cest { get; private set; } = String.Empty;
        public string prefixEanGtin { get; private set; } = String.Empty;
        public string measuredUnit { get; private set; } = String.Empty;
        public string size { get; private set; } = String.Empty;
        public decimal? weight { get; private set; }
        public bool? isActive { get; private set; }
        public DateTime? insertDate { get; private set; }
        public bool? isFinishedProduct { get; private set; }
        public bool? isRawMaterial { get; private set; }
        public bool? isBulkMaterial { get; private set; }
        public bool? isOwnProduction { get; private set; }
        public bool? isKit { get; private set; }
        public int? integrationProductCode { get; private set; }
        public decimal? minimumStockAmount { get; private set; }
        public decimal? maximumStockAmount { get; private set; }
        public decimal? idealStockAmount { get; private set; }
        public DateTime? salesStartDate { get; private set; }
        public DateTime? salesEndDate { get; private set; }
        public DateTime? billingForecastDate { get; private set; }
        public bool? isBlocked { get; private set; }
        public string balanceValidationType { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Barcode> barCodes { get; private set; } = [];

        [SkipProperty]
        public List<Classification> classifications { get; private set; } = [];

        [SkipProperty]
        public List<AdditionalField> additionalFields { get; private set; } = [];

        [SkipProperty]
        public List<Supplier> suppliers { get; private set; } = [];

        [SkipProperty]
        public List<Manufacturer> manufacturers { get; private set; } = [];

        [SkipProperty]
        public List<ReferenceCodeSequence> referenceCodeSequences { get; private set; } = [];

        [SkipProperty]
        public List<WebData> webData { get; private set; } = [];

        [SkipProperty]
        public List<WebDetail> webDetails { get; private set; } = [];

        [SkipProperty]
        public List<BranchProductBlocked> branchesProductBlocked { get; private set; } = [];

        [SkipProperty]
        public List<ConservationInstruction> conservationInstructions { get; private set; } = [];

        public Product() { }

        public Product(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, string colorCode, Dtos.Reads.Products.Product dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.colorCode = colorCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            sku = ConverterValidatorsExtensions.StringLengthValidation(dto.sku, 60);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
            ncm = ConverterValidatorsExtensions.StringLengthValidation(dto.ncm, 60);
            ipi = NullableConverters.ToNullableDecimal(dto.ipi);
            cst = ConverterValidatorsExtensions.StringLengthValidation(dto.cst, 60);
            cest = ConverterValidatorsExtensions.StringLengthValidation(dto.cest, 60);
            prefixEanGtin = ConverterValidatorsExtensions.StringLengthValidation(dto.prefixEanGtin, 60);
            measuredUnit = ConverterValidatorsExtensions.StringLengthValidation(dto.measuredUnit, 60);
            size = ConverterValidatorsExtensions.StringLengthValidation(dto.size, 60);
            weight = NullableConverters.ToNullableDecimal(dto.weight);
            isActive = NullableConverters.ToNullableBoolean(dto.isActive);
            insertDate = NullableConverters.ToNullableDateTime(dto.insertDate);
            isFinishedProduct = NullableConverters.ToNullableBoolean(dto.isFinishedProduct);
            isRawMaterial = NullableConverters.ToNullableBoolean(dto.isRawMaterial);
            isBulkMaterial = NullableConverters.ToNullableBoolean(dto.isBulkMaterial);
            isOwnProduction = NullableConverters.ToNullableBoolean(dto.isOwnProduction);
            isKit = NullableConverters.ToNullableBoolean(dto.isKit);
            integrationProductCode = NullableConverters.ToNullableInt32(dto.integrationProductCode);
            minimumStockAmount = NullableConverters.ToNullableDecimal(dto.minimumStockAmount);
            maximumStockAmount = NullableConverters.ToNullableDecimal(dto.maximumStockAmount);
            idealStockAmount = NullableConverters.ToNullableDecimal(dto.idealStockAmount);
            salesStartDate = NullableConverters.ToNullableDateTime(dto.SalesStartDate);
            salesEndDate = NullableConverters.ToNullableDateTime(dto.SalesEndDate);
            billingForecastDate = NullableConverters.ToNullableDateTime(dto.BillingForecastDate);
            isBlocked = NullableConverters.ToNullableBoolean(dto.isBlocked);
            balanceValidationType = ConverterValidatorsExtensions.StringLengthValidation(dto.balanceValidationType, 60);

            // repetidos violariam a PK de cada filha (chaves conferidas no teste real, src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4)
            barCodes.AddRange((dto.barCodes ?? []).Select(x => new Barcode(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
            classifications.AddRange((dto.classifications ?? []).Select(x => new Classification(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.typeCode).Select(g => g.Last()));
            additionalFields.AddRange((dto.additionalFields ?? []).Select(x => new AdditionalField(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
            suppliers.AddRange((dto.suppliers ?? []).Select(x => new Supplier(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
            manufacturers.AddRange((dto.manufacturers ?? []).Select(x => new Manufacturer(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
            referenceCodeSequences.AddRange((dto.referenceCodeSequences ?? []).Select(x => new ReferenceCodeSequence(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => (x.referenceId, x.parentCode)).Select(g => g.Last()));
            webData.AddRange((dto.webData ?? []).Select(x => new WebData(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.index).Select(g => g.Last()));
            webDetails.AddRange((dto.webDetail ?? []).Select(x => new WebDetail(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.descriptionType).Select(g => g.Last()));
            branchesProductBlocked.AddRange((dto.branchesProductBlocked ?? []).Select(x => new BranchProductBlocked(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.branchCode).Select(g => g.Last()));
            conservationInstructions.AddRange((dto.conservationInstructions ?? []).Select(x => new ConservationInstruction(doc_company, integrated_or_updated_at, branchInfoCode, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
        }
    }
}
