using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaTagsValidator : AbstractValidator<B2CConsultaTags>
    {
        public B2CConsultaTagsValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_pedido_item)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pedido_item));

            RuleFor(x => x.descricao)
                .MaximumLength(300)
                .WithMessage("O campo 'descricao' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
