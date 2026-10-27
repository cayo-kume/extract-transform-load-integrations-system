using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script052_Create_LinxMicrovix_Orders_Tables
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaDificuldadeValidator : AbstractValidator<LinxPedidosVendaChecklistEntregaDificuldade>
    {
        public LinxPedidosVendaChecklistEntregaDificuldadeValidator()
        {
            RuleFor(x => x.id_checklist_entrega_dificuldade)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_checklist_entrega_dificuldade' é obrigatório.")
                .MaximumLength(1000)
                .WithMessage("O campo 'id_checklist_entrega_dificuldade' deve ter no máximo 1000 caracteres.");

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
