using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosDepositosValidator : AbstractValidator<B2CConsultaProdutosDepositos>
    {
        public B2CConsultaProdutosDepositosValidator()
        {
            RuleFor(x => x.id_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_deposito));

            RuleFor(x => x.nome_deposito)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_deposito' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_deposito));

            RuleFor(x => x.disponivel)
                .MaximumLength(1)
                .WithMessage("O campo 'disponivel' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.disponivel));

            RuleFor(x => x.disponivel_transferencia)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.disponivel_transferencia));

            RuleFor(x => x.disponivel_franquias)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.disponivel_franquias));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
