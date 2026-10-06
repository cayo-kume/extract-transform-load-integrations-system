using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaLinhasValidator : AbstractValidator<B2CConsultaLinhas>
    {
        public B2CConsultaLinhasValidator()
        {
            RuleFor(x => x.codigo_linha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_linha));

            RuleFor(x => x.nome_linha)
                .MaximumLength(30)
                .WithMessage("O campo 'nome_linha' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_linha));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.setores)
                .MaximumLength(250)
                .WithMessage("O campo 'setores' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.setores));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
