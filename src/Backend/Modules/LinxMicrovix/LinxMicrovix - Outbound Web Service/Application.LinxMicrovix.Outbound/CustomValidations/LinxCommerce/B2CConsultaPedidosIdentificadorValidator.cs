using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPedidosIdentificadorValidator : AbstractValidator<B2CConsultaPedidosIdentificador>
    {
        public B2CConsultaPedidosIdentificadorValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.identificador)
                .Must(value => Guid.TryParse(value, out _))
                .WithMessage("O campo 'identificador' deve ser um GUID válido.")
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.id_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_venda));

            RuleFor(x => x.order_id)
                .MaximumLength(40)
                .WithMessage("O campo 'order_id' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.id_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cliente));

            RuleFor(x => x.valor_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_frete));

            RuleFor(x => x.data_origem)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_origem));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
