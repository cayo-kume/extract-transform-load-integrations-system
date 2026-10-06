using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPedidosVendaChecklistEntregaDificuldadeValidator : AbstractValidator<LinxPedidosVendaChecklistEntregaDificuldade>
    {
        public LinxPedidosVendaChecklistEntregaDificuldadeValidator()
        {
            RuleFor(x => x.id_checklist_entrega_dificuldades).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_checklist_entrega_dificuldades));
            RuleFor(x => x.descricao).MaximumLength(100).WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}