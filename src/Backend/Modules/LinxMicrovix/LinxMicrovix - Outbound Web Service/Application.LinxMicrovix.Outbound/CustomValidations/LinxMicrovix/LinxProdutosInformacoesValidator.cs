using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosInformacoesValidator : AbstractValidator<LinxProdutosInformacoes>
    {
        public LinxProdutosInformacoesValidator()
        {
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
        }
    }
}