using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxListagemBalancoValidator : AbstractValidator<LinxListagemBalanco>
    {
        public LinxListagemBalancoValidator()
        {
            RuleFor(x => x.id_balanco).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_balanco));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.nome_arquivo).MaximumLength(50).WithMessage("O campo 'nome_arquivo' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_arquivo));
            RuleFor(x => x.processado).MaximumLength(1).WithMessage("O campo 'processado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.processado));
            RuleFor(x => x.data_ultimo_processamento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_ultimo_processamento));
            RuleFor(x => x.qtde_produtos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_produtos));
            RuleFor(x => x.qtde_itens).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_itens));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}