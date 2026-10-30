using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxMovimentoRemessasItensValidator : AbstractValidator<LinxMovimentoRemessasItens>
    {
        public LinxMovimentoRemessasItensValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_remessas_itens)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_remessas_itens' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_remessas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas));

            RuleFor(x => x.transacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao));

            RuleFor(x => x.qtde_total)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_total));

            RuleFor(x => x.qtde_venda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_venda));

            RuleFor(x => x.qtde_devolvido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_devolvido));

            RuleFor(x => x.qtde_pendente)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_pendente));

            RuleFor(x => x.qtde_pendente_pagamento)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_pendente_pagamento));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
