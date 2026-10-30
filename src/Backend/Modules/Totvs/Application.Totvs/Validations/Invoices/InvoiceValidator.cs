using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.branchCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'branchCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.branchCnpj));

            RuleFor(x => x.invoiceSequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'invoiceSequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.invoiceDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'invoiceDate' é obrigatório.")
                .MustBeValidDateTime();

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(100)
                .WithMessage("O campo 'personName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.invoiceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceCode));

            RuleFor(x => x.serialCode)
                .MaximumLength(3)
                .WithMessage("O campo 'serialCode' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialCode));

            RuleFor(x => x.invoiceStatus)
                .MaximumLength(20)
                .WithMessage("O campo 'invoiceStatus' deve ter no máximo 20 caracteres.")
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
                .MaximumLength(20)
                .WithMessage("O campo 'inclusionComponentCode' deve ter no máximo 20 caracteres.")
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
                .MaximumLength(100)
                .WithMessage("O campo 'idDocumentPDV' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.idDocumentPDV));

            RuleFor(x => x.userCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.userCode));

            RuleFor(x => x.origin)
                .MaximumLength(20)
                .WithMessage("O campo 'origin' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.origin));

            RuleFor(x => x.documentType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documentType));

            RuleFor(x => x.operationType)
                .MaximumLength(20)
                .WithMessage("O campo 'operationType' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationType));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.operatioName)
                .MaximumLength(100)
                .WithMessage("O campo 'operatioName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operatioName));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.releaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.releaseDate));

            RuleFor(x => x.exitTime)
                .MaximumLength(10)
                .WithMessage("O campo 'exitTime' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.exitTime));

            RuleFor(x => x.lastchangeDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastchangeDate));

            // sem ele a nota é regravada em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

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

            RuleFor(x => x.eletronic!)
                .SetValidator(new ElectronicValidator())
                .When(x => x.eletronic is not null);

            RuleFor(x => x.ecf!)
                .SetValidator(new EcfValidator())
                .When(x => x.ecf is not null);

            RuleFor(x => x.sat!)
                .SetValidator(new SatValidator())
                .When(x => x.sat is not null);

            RuleFor(x => x.shippingCompany!)
                .SetValidator(new ShippingCompanyValidator())
                .When(x => x.shippingCompany is not null);

            RuleFor(x => x.person!)
                .SetValidator(new PersonValidator())
                .When(x => x.person is not null);

            RuleFor(x => x.productionOrder!)
                .SetValidator(new ProductionOrderValidator())
                .When(x => x.productionOrder is not null);

            RuleForEach(x => x.items)
                .SetValidator(new ItemValidator());

            RuleForEach(x => x.payments)
                .SetValidator(new PaymentValidator());

            RuleForEach(x => x.salesOrder)
                .SetValidator(new SalesOrderValidator());

            RuleForEach(x => x.observationNF)
                .SetValidator(new ObservationNFValidator());

            RuleForEach(x => x.referencedTaxInvoice)
                .SetValidator(new ReferencedTaxInvoiceValidator());
        }
    }
}
