using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaGrade1Validator : AbstractValidator<B2CConsultaGrade1>
    {
        public B2CConsultaGrade1Validator()
        {
            RuleFor(x => x.codigo_grade1)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_grade1));

            RuleFor(x => x.nome_grade1)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_grade1' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_grade1));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
