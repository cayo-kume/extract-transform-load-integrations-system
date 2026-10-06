using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaSetoresValidator : AbstractValidator<B2CConsultaSetores>
    {
        public B2CConsultaSetoresValidator()
        {
            RuleFor(x => x.codigo_setor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_setor));

            RuleFor(x => x.nome_setor)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_setor' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_setor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
