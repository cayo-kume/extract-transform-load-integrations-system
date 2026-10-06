using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaGrade2Validator : AbstractValidator<B2CConsultaGrade2>
    {
        public B2CConsultaGrade2Validator()
        {
            RuleFor(x => x.codigo_grade2)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_grade2));

            RuleFor(x => x.nome_grade2)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_grade2' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_grade2));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
