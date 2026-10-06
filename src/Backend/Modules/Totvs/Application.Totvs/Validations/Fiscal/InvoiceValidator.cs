using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class InvoiceValidator : AbstractValidator<Item>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.branchCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'branchCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.branchCnpj));

            RuleFor(x => x.invoiceSequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceSequence));

            RuleFor(x => x.invoiceDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.invoiceDate));

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(250)
                .WithMessage("O campo 'personName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.invoiceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceCode));

            RuleFor(x => x.serialCode)
                .MaximumLength(60)
                .WithMessage("O campo 'serialCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialCode));

            RuleFor(x => x.invoiceStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'invoiceStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoiceStatus));

            RuleFor(x => x.transactionBranchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transactionBranchCode));

            RuleFor(x => x.transactionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.transactionDate));

            RuleFor(x => x.transactionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transactionCode));

            RuleFor(x => x.inclusionComponentCode)
                .MaximumLength(60)
                .WithMessage("O campo 'inclusionComponentCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inclusionComponentCode));

            RuleFor(x => x.peripheralPdvCode)
                .MaximumLength(60)
                .WithMessage("O campo 'peripheralPdvCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.peripheralPdvCode));

            RuleFor(x => x.versionPdv)
                .MaximumLength(60)
                .WithMessage("O campo 'versionPdv' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.versionPdv));

            RuleFor(x => x.mobileVersion)
                .MaximumLength(60)
                .WithMessage("O campo 'mobileVersion' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.mobileVersion));

            RuleFor(x => x.idDocumentPDV)
                .MaximumLength(60)
                .WithMessage("O campo 'idDocumentPDV' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.idDocumentPDV));

            RuleFor(x => x.userCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.userCode));

            RuleFor(x => x.origin)
                .MaximumLength(60)
                .WithMessage("O campo 'origin' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.origin));

            RuleFor(x => x.documentType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documentType));

            RuleFor(x => x.operationType)
                .MaximumLength(60)
                .WithMessage("O campo 'operationType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationType));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.operatioName)
                .MaximumLength(60)
                .WithMessage("O campo 'operatioName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operatioName));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.releaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.releaseDate));

            RuleFor(x => x.exitTime)
                .MaximumLength(60)
                .WithMessage("O campo 'exitTime' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.exitTime));

            RuleFor(x => x.lastchangeDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastchangeDate));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.paymentConditionName)
                .MaximumLength(60)
                .WithMessage("O campo 'paymentConditionName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionName));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.productValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productValue));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.shippingValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shippingValue));

            RuleFor(x => x.insuranceValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.insuranceValue));

            RuleFor(x => x.ipiValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipiValue));

            RuleFor(x => x.baseIcmsValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.baseIcmsValue));

            RuleFor(x => x.icmsValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icmsValue));

            RuleFor(x => x.icmsSubStValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icmsSubStValue));

            RuleFor(x => x.totalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalValue));

            RuleFor(x => x.terminalCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.terminalCode));

            RuleFor(x => x.sellerCpf)
                .MaximumLength(14)
                .WithMessage("O campo 'sellerCpf' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerCpf));

            RuleFor(x => x.observationNFE)
                .MaximumLength(8000)
                .WithMessage("O campo 'observationNFE' deve ter no máximo 8000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observationNFE));

            RuleFor(x => x.eletronic)
                .SetValidator(new EletronicValidator());
            
            RuleFor(x => x.ecf)
                .SetValidator(new EcfValidator());
            
            RuleFor(x => x.sat)
                .SetValidator(new SatValidator());
            
            RuleFor(x => x.shippingCompany)
                .SetValidator(new ShippingCompanyValidator());
            
            RuleFor(x => x.person)
                .SetValidator(new PersonValidator());

            RuleFor(x => x.productionOrder)
                .SetValidator(new ProductionOrderValidator());

            RuleForEach(x => x.salesOrder)
                .SetValidator(new Application.Totvs.Validations.Fiscal.SalesOrderValidator());

            RuleForEach(x => x.items)
                .SetValidator(new ProductItemValidator());

            RuleForEach(x => x.payments)
                .SetValidator(new PaymentValidator());

            RuleForEach(x => x.observationNF)
                .SetValidator(new ObservationNFValidator());

            RuleForEach(x => x.referencedTaxInvoice)
                .SetValidator(new ReferencedTaxInvoiceValidator());
        }
    }
}
