using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;

namespace Domain.Totvs.Entities.Product
{
    public class Product
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int code { get; private set; }
        public string sku { get; private set; }
        public string name { get; private set; }
        public string ncm { get; private set; }
        public decimal ipi { get; private set; }
        public string cst { get; private set; }
        public string cest { get; private set; }
        public string prefixEanGtin { get; private set; }
        public string measuredUnit { get; private set; }
        public string size { get; private set; }
        public decimal weight { get; private set; }
        public bool isActive { get; private set; }
        public DateTime insertDate { get; private set; }
        public bool isFinishedProduct { get; private set; }
        public bool isRawMaterial { get; private set; }
        public bool isBulkMaterial { get; private set; }
        public bool isOwnProduction { get; private set; }
        public decimal minimumStockAmount { get; private set; }
        public decimal maximumStockAmount { get; private set; }
        public decimal idealStockAmount { get; private set; }
        public DateTime salesStartDate { get; private set; }
        public DateTime salesEndDate { get; private set; }
        public bool isBlocked { get; private set; }
        public string balanceValidationType { get; private set; }

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
        public List<WebData> webDatas { get; private set; } = [];

        [SkipProperty]
        public List<WebDetail> webDetails { get; private set; } = [];

        [SkipProperty]
        public List<BranchesProductBlocked> branchesProductBlocked { get; private set; } = [];

        [SkipProperty]
        public List<ConservationInstruction> conservationInstructions { get; private set; } = [];

        public Product() { }

        public Product(int referenceId, string colorCode, Dtos.Product.Product productDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.code);

            if (productDto.sku?.Length > 60)
                sku = productDto.sku.Substring(0, 60);
            else if (String.IsNullOrEmpty(productDto.sku))
                sku = productDto.code;
            else
                sku = productDto.sku;

            name = productDto.name?.Length > 250 ? productDto.name.Substring(0, 250) : productDto.name;
            ncm = productDto.ncm?.Length > 60 ? productDto.ncm.Substring(0, 60) : productDto.ncm;
            ipi = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.ipi);
            cst = productDto.cst?.Length > 60 ? productDto.cst.Substring(0, 60) : productDto.cst;
            cest = productDto.cest?.Length > 60 ? productDto.cest.Substring(0, 60) : productDto.cest;
            prefixEanGtin = productDto.prefixEanGtin?.Length > 60 ? productDto.prefixEanGtin.Substring(0, 60) : productDto.prefixEanGtin;
            measuredUnit = productDto.measuredUnit?.Length > 60 ? productDto.measuredUnit.Substring(0, 60) : productDto.measuredUnit;
            size = productDto.size?.Length > 60 ? productDto.size.Substring(0, 60) : productDto.size;
            weight = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.weight);
            isActive = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isActive);
            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.insertDate);
            isFinishedProduct = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isFinishedProduct);
            isRawMaterial = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isRawMaterial);
            isBulkMaterial = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isBulkMaterial);
            isOwnProduction = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isOwnProduction);
            minimumStockAmount = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.minimumStockAmount);
            maximumStockAmount = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.maximumStockAmount);
            idealStockAmount = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.idealStockAmount);
            salesStartDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.SalesStartDate);
            salesEndDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.SalesEndDate);
            isBlocked = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.isBlocked);
            balanceValidationType = productDto.balanceValidationType?.Length > 60 ? productDto.balanceValidationType.Substring(0, 60) : productDto.balanceValidationType;

            productDto.barCodes?.ForEach(barCode =>
                barCodes.Add(new Barcode(referenceId, colorCode, code, sku, barCode)
            ));

            productDto.classifications?.ForEach(classification =>
                classifications.Add(new Classification(referenceId, colorCode, code, sku, classification)
            ));

            productDto.additionalFields?.ForEach(additionalField =>
                additionalFields.Add(new AdditionalField(referenceId, colorCode, code, sku, additionalField)
            ));

            productDto.suppliers?.ForEach(supplier =>
                suppliers.Add(new Supplier(referenceId, colorCode, code, sku, supplier)
            ));

            productDto.manufacturers?.ForEach(manufacturer =>
                manufacturers.Add(new Manufacturer(referenceId, colorCode, code, sku, manufacturer)
            ));

            productDto.referenceCodeSequences?.ForEach(referenceCodeSequence =>
                referenceCodeSequences.Add(new ReferenceCodeSequence(referenceId, colorCode, code, sku, referenceCodeSequence)
            ));

            for (int i = 0; i < productDto.webData?.Count; i++)
            {
                webDatas.Add(new WebData(i + 1, referenceId, colorCode, code, sku, productDto.webData[i]));
            }

            productDto.webDetail?.ForEach(webDetail =>
                webDetails.Add(new WebDetail(referenceId, colorCode, code, sku, webDetail)
            ));

            productDto.branchesProductBlocked?.ForEach(brancheProductBlocked =>
                branchesProductBlocked.Add(new BranchesProductBlocked(referenceId, colorCode, code, sku, brancheProductBlocked)
            ));

            productDto.conservationInstructions?.ForEach(conservationInstruction =>
                conservationInstructions.Add(new ConservationInstruction(referenceId, colorCode, code, sku, conservationInstruction)
            ));
        }
    }
}
