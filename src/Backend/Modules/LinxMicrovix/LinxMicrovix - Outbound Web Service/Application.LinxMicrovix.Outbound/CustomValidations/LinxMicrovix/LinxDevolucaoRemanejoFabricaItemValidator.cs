using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxDevolucaoRemanejoFabricaItemValidator : AbstractValidator<LinxDevolucaoRemanejoFabricaItem>
    {
        public LinxDevolucaoRemanejoFabricaItemValidator()
        {
            RuleFor(x => x.id_devolucao_remanejo_fabrica_item).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_devolucao_remanejo_fabrica_item));
            RuleFor(x => x.id_devolucao_remanejo_fabrica).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_devolucao_remanejo_fabrica));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.codigo_produto_franqueadora).MaximumLength(50).WithMessage("O campo 'codigo_produto_franqueadora' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_produto_franqueadora));
            RuleFor(x => x.quantidade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.codebar).MaximumLength(20).WithMessage("O campo 'codebar' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codebar));
            RuleFor(x => x.serial).MaximumLength(50).WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.serial));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
