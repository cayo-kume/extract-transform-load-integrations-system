using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersShipmentsPackages (scripts de tabelas do módulo)
    /// </summary>
    public class OrderPackageValidator : AbstractValidator<OrderPackage>
    {
        public OrderPackageValidator()
        {
            RuleFor(x => x.OrderPackageID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderPackageID' é obrigatório.")
                .MustBeValidGuid()
                .WithMessage("O campo 'OrderPackageID' deve ser um Guid válido.");

            RuleFor(x => x.DeliveryMethodID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DeliveryMethodID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryMethodID));

            RuleFor(x => x.PackageNumber)
                .MaximumLength(50)
                .WithMessage("O campo 'PackageNumber' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PackageNumber));

            RuleFor(x => x.TrackingNumber)
                .MaximumLength(100)
                .WithMessage("O campo 'TrackingNumber' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.TrackingNumber));

            RuleFor(x => x.TrackingNumberUrl)
                .MaximumLength(1000)
                .WithMessage("O campo 'TrackingNumberUrl' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.TrackingNumberUrl));

            RuleFor(x => x.ShippedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ShippedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ShippedDate));

            RuleFor(x => x.ShippedBy)
                .MaximumLength(250)
                .WithMessage("O campo 'ShippedBy' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ShippedBy));

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));

            RuleFor(x => x.PackageType)
                .MaximumLength(50)
                .WithMessage("O campo 'PackageType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PackageType));

            RuleFor(x => x.Source)
                .MaximumLength(50)
                .WithMessage("O campo 'Source' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Source));

            RuleFor(x => x.InsuranceAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'InsuranceAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.InsuranceAmount));

            RuleFor(x => x.Height)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Height' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Height));

            RuleFor(x => x.Width)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Width' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Width));

            RuleFor(x => x.Length)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Length' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Length));

            RuleFor(x => x.Weight)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Weight' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Weight));
        }
    }
}
