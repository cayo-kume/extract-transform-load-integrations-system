using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxMovimentoOrdemServicoExternaValidator : AbstractValidator<LinxMovimentoOrdemServicoExterna>
    {
        public LinxMovimentoOrdemServicoExternaValidator()
        {
            RuleFor(x => x.id_movimento_ordem_servico_externa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_movimento_ordem_servico_externa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.codigo_ordem_servico)
                .MaximumLength(20)
                .WithMessage("O campo 'codigo_ordem_servico' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_ordem_servico));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
