using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxOrdemServicoExternaDavValidator : AbstractValidator<LinxOrdemServicoExternaDav>
    {
        public LinxOrdemServicoExternaDavValidator()
        {
            RuleFor(x => x.id_vendas_pos_tmp_ordem_servico_externa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_vendas_pos_tmp_ordem_servico_externa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));

            RuleFor(x => x.codigo_ordem_servico_externa)
                .MaximumLength(20)
                .WithMessage("O campo 'codigo_ordem_servico_externa' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_ordem_servico_externa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
