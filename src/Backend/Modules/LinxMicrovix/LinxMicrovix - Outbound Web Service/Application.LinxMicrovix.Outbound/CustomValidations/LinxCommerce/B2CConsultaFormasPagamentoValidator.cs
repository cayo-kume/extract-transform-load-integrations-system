using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaFormasPagamentoValidator : AbstractValidator<B2CConsultaFormasPagamento>
    {
        public B2CConsultaFormasPagamentoValidator()
        {
            RuleFor(x => x.cod_forma_pgto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));

            RuleFor(x => x.forma_pgto)
                .MaximumLength(50)
                .WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
