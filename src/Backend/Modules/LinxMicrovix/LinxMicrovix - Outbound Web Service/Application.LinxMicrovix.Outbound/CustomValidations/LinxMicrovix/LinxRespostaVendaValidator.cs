using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxRespostaVendaValidator : AbstractValidator<LinxRespostaVenda>
    {
        public LinxRespostaVendaValidator()
        {
            RuleFor(x => x.id_resposta_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_resposta_venda));
            RuleFor(x => x.id_pergunta_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_pergunta_venda));
            RuleFor(x => x.descricao_resposta).MaximumLength(100).WithMessage("O campo 'descricao_resposta' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_resposta));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}