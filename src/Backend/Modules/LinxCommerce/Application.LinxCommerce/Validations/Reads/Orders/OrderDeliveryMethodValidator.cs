using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersDeliveryMethods (scripts de tabelas do módulo)
    /// </summary>
    public class OrderDeliveryMethodValidator : AbstractValidator<OrderDeliveryMethod>
    {
        public OrderDeliveryMethodValidator()
        {
            RuleFor(x => x.OrderDeliveryMethodID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderDeliveryMethodID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderDeliveryMethodID' deve ser um número inteiro.");

            RuleFor(x => x.DeliveryMethodID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DeliveryMethodID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryMethodID));

            RuleFor(x => x.DeliveryGroupID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DeliveryGroupID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryGroupID));

            RuleFor(x => x.DeliveryMethodAlias)
                .MaximumLength(100)
                .WithMessage("O campo 'DeliveryMethodAlias' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryMethodAlias));

            RuleFor(x => x.DeliveryMethodType)
                .MaximumLength(50)
                .WithMessage("O campo 'DeliveryMethodType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryMethodType));

            RuleFor(x => x.DeliveryLogisticType)
                .MaximumLength(50)
                .WithMessage("O campo 'DeliveryLogisticType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryLogisticType));

            RuleFor(x => x.LogisticOptionId)
                .MaximumLength(100)
                .WithMessage("O campo 'LogisticOptionId' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.LogisticOptionId));

            RuleFor(x => x.LogisticOptionName)
                .MaximumLength(250)
                .WithMessage("O campo 'LogisticOptionName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.LogisticOptionName));

            RuleFor(x => x.LogisticContractId)
                .MaximumLength(100)
                .WithMessage("O campo 'LogisticContractId' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.LogisticContractId));

            RuleFor(x => x.LogisticContractName)
                .MaximumLength(250)
                .WithMessage("O campo 'LogisticContractName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.LogisticContractName));

            RuleFor(x => x.Amount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Amount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Amount));

            RuleFor(x => x.ETA)
                .MaximumLength(50)
                .WithMessage("O campo 'ETA' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ETA));

            RuleFor(x => x.ETADays)
                .MustBeValidInt32()
                .WithMessage("O campo 'ETADays' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ETADays));

            RuleFor(x => x.IntegrationID)
                .MustBeValidInt64()
                .WithMessage("O campo 'IntegrationID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.IntegrationID));

            RuleFor(x => x.ScheduleShiftID)
                .MustBeValidInt64()
                .WithMessage("O campo 'ScheduleShiftID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ScheduleShiftID));

            RuleFor(x => x.ScheduleDisplayName)
                .MaximumLength(250)
                .WithMessage("O campo 'ScheduleDisplayName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ScheduleDisplayName));

            RuleFor(x => x.ScheduleTax)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'ScheduleTax' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.ScheduleTax));

            RuleFor(x => x.ScheduleDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ScheduleDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ScheduleDate));

            RuleFor(x => x.PointOfSaleID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PointOfSaleID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PointOfSaleID));

            RuleFor(x => x.PointOfSaleIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'PointOfSaleIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PointOfSaleIntegrationID));

            RuleFor(x => x.PointOfSaleName)
                .MaximumLength(250)
                .WithMessage("O campo 'PointOfSaleName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PointOfSaleName));

            RuleFor(x => x.ExternalID)
                .MaximumLength(100)
                .WithMessage("O campo 'ExternalID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ExternalID));

            RuleFor(x => x.WarehouseID)
                .MustBeValidInt32()
                .WithMessage("O campo 'WarehouseID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.WarehouseID));

            RuleFor(x => x.WarehouseIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'WarehouseIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WarehouseIntegrationID));

            RuleFor(x => x.DockID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DockID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DockID));

            RuleFor(x => x.CarrierName)
                .MaximumLength(250)
                .WithMessage("O campo 'CarrierName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CarrierName));

            RuleFor(x => x.DeliveryEstimatedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'DeliveryEstimatedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryEstimatedDate));
        }
    }
}
