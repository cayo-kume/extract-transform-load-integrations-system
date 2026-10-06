using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPedidosVendaChecklistEntregaArmazenamentoValidator : AbstractValidator<LinxPedidosVendaChecklistEntregaArmazenamento>
    {
        public LinxPedidosVendaChecklistEntregaArmazenamentoValidator()
        {
            RuleFor(x => x.id_checklist_entrega_armazenamento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_checklist_entrega_armazenamento));
            RuleFor(x => x.descricao).MaximumLength(100).WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
