using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOrdensServicoProdutosValidator : AbstractValidator<LinxOrdensServicoProdutos>
    {
        public LinxOrdensServicoProdutosValidator()
        {
            RuleFor(x => x.id_ordservprod).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ordservprod));
            RuleFor(x => x.cod_produto_serv).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto_serv));
            RuleFor(x => x.numero_os).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.numero_os));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
