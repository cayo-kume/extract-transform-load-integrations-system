using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class Tax
    {
        public int invoiceSequence { get; private set; }
        public int invoiceItemsSequence { get; private set; }

        public int code { get; private set; }

        public string name { get; private set; }
        public string cst { get; private set; }
        public string benefitCode { get; private set; }
        
        public decimal taxPercentage { get; private set; }
        public decimal calculationBasisPercentage { get; private set; }
        public decimal calculationBasisDiscountPercentage { get; private set; }
        public decimal calculationBasisValue { get; private set; }
        public decimal freeValue { get; private set; }
        public decimal otherValue { get; private set; }
        public decimal taxValue { get; private set; }
        public decimal unencumberedValue { get; private set; }
        public decimal unencumberedReason { get; private set; }
        public decimal deferredBaseValue { get; private set; }

        public Tax() { }

        public Tax(Domain.Totvs.Dtos.Fiscal.Tax taxDto, int invoiceSequence, int invoiceItemsSequence)
        {
            this.invoiceSequence = invoiceSequence;
            this.invoiceItemsSequence = invoiceItemsSequence;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(taxDto.code);

            name = taxDto.name?.Length > 60 ? taxDto.name?.Substring(0, 60) : taxDto.name;
            cst = taxDto.cst?.Length > 60 ? taxDto.cst?.Substring(0, 60) : taxDto.cst;
            benefitCode = taxDto.benefitCode?.Length > 60 ? taxDto.benefitCode?.Substring(0, 60) : taxDto.benefitCode;

            taxPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.taxPercentage);
            calculationBasisPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.calculationBasisPercentage);
            calculationBasisDiscountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.calculationBasisDiscountPercentage);
            calculationBasisValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.calculationBasisValue);
            freeValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.freeValue);
            otherValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.otherValue);
            taxValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.taxValue);
            unencumberedValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.unencumberedValue);
            unencumberedReason = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.unencumberedReason);
            deferredBaseValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(taxDto.deferredBaseValue);
        }
    }
}
