using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosDepositosValidator : AbstractValidator<LinxProdutosDepositos>
    {
        public LinxProdutosDepositosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_deposito));
            RuleFor(x => x.nome_deposito).MaximumLength(50).WithMessage("O campo 'nome_deposito' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_deposito));
            RuleFor(x => x.disponivel).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.disponivel));
            RuleFor(x => x.disponivel_transferencia).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.disponivel_transferencia));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
