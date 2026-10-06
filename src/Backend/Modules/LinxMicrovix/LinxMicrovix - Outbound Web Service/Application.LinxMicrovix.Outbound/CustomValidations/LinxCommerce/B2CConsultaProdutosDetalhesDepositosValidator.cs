using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosDetalhesDepositosValidator : AbstractValidator<B2CConsultaProdutosDetalhesDepositos>
    {
        public B2CConsultaProdutosDetalhesDepositosValidator()
        {
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_deposito));
            RuleFor(x => x.saldo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.saldo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.deposito));
            RuleFor(x => x.tempo_preparacao_estoque).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tempo_preparacao_estoque));
        }
    }
}
