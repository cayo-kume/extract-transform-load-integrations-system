using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaMarcasValidator : AbstractValidator<B2CConsultaMarcas>
    {
        public B2CConsultaMarcasValidator()
        {
            RuleFor(x => x.codigo_marca)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_marca));

            RuleFor(x => x.nome_marca)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_marca' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_marca));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.linhas)
                .MaximumLength(250)
                .WithMessage("O campo 'linhas' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.linhas));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
