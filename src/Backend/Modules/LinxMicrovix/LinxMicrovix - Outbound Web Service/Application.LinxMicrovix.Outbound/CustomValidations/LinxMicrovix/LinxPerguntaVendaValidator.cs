using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPerguntaVendaValidator : AbstractValidator<LinxPerguntaVenda>
    {
        public LinxPerguntaVendaValidator()
        {
            RuleFor(x => x.id_pergunta_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_pergunta_venda));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.descricao_pergunta).MaximumLength(200).WithMessage("O campo 'descricao_pergunta' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_pergunta));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}