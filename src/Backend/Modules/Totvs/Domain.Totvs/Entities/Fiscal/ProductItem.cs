using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class ProductItem
    {
        public int invoiceSequence { get; private set; }

        //item
        public int sequence { get; private set; }

        public string code { get; private set; }
        public string name { get; private set; }
        public string ncm { get; private set; }
        public string cfop { get; private set; }
        public string measureUnit { get; private set; }
        public string cest { get; private set; }
        public string additionalItemInformation { get; private set; }

        public decimal quantity { get; private set; }
        public decimal grossValue { get; private set; }
        public decimal discountValue { get; private set; }
        public decimal netValue { get; private set; }
        public decimal unitGrossValue { get; private set; }
        public decimal unitDiscountValue { get; private set; }
        public decimal unitNetValue { get; private set; }
        public decimal additionalValue { get; private set; }
        public decimal freightValue { get; private set; }
        public decimal insuranceValue { get; private set; }

        //rate differential
        public decimal calculationBaseValue { get; private set; }
        public decimal internalRate { get; private set; }
        public decimal interstateRate { get; private set; }
        public decimal fcpRate { get; private set; }
        public decimal fcpValue { get; private set; }
        public decimal recipientRateDifferentialValue { get; private set; }
        public decimal senderRateDifferentialValue { get; private set; }

        [SkipProperty]
        public List<Product> products { get; private set; } = new List<Product>();

        [SkipProperty]
        public List<Tax> taxes { get; private set; } = new List<Tax>();

        [SkipProperty]
        public List<BarCode> barCodes { get; private set; } = new List<BarCode>();

        public ProductItem() { }

        public ProductItem(Domain.Totvs.Dtos.Fiscal.ProductItem itemDto, int invoiceSequence)
        {
            this.invoiceSequence = invoiceSequence;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(itemDto.sequence);

            code = itemDto.code?.Length > 44 ? itemDto.code?.Substring(0, 44) : itemDto.code;
            name = itemDto.name?.Length > 250 ? itemDto.name?.Substring(0, 250) : itemDto.name;
            ncm = itemDto.ncm?.Length > 60 ? itemDto.ncm?.Substring(0, 60) : itemDto.ncm;
            cfop = itemDto.cfop?.Length > 20 ? itemDto.cfop?.Substring(0, 20) : itemDto.cfop;
            measureUnit = itemDto.measureUnit?.Length > 60 ? itemDto.measureUnit?.Substring(0, 60) : itemDto.measureUnit;
            cest = itemDto.cest?.Length > 60 ? itemDto.cest?.Substring(0, 60) : itemDto.cest;
            additionalItemInformation = itemDto.additionalItemInformation?.Length > 60 ? itemDto.additionalItemInformation?.Substring(0, 60) : itemDto.additionalItemInformation;

            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.quantity);
            grossValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.grossValue);
            discountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.discountValue);
            netValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.netValue);
            unitGrossValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.unitGrossValue);
            unitDiscountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.unitDiscountValue);
            unitNetValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.unitNetValue);
            additionalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.additionalValue);
            freightValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.freightValue);
            insuranceValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.insuranceValue);

            //rate differential
            calculationBaseValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.calculationBaseValue);
            internalRate = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.internalRate);
            interstateRate = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.interstateRate);
            fcpRate = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.fcpRate);
            fcpValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.fcpValue);
            recipientRateDifferentialValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.recipientRateDifferentialValue);
            senderRateDifferentialValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(itemDto.rateDifferential?.senderRateDifferentialValue);

            itemDto.products?.ForEach(product =>
                products.Add(new Product(product, invoiceSequence, sequence)
            ));

            itemDto.taxes?.ForEach(tax =>
                taxes.Add(new Tax(tax, invoiceSequence, sequence)
            ));

            itemDto.barCodes?.ForEach(barCode =>
                barCodes.Add(new BarCode(barCode, invoiceSequence, sequence)
            ));
        }
    }
}
