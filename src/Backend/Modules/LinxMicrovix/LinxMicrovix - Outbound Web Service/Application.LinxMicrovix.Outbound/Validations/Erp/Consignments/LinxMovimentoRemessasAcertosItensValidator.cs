using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxMovimentoRemessasAcertosItensValidator : AbstractValidator<LinxMovimentoRemessasAcertosItens>
    {
        public LinxMovimentoRemessasAcertosItensValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_remessas_acertos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas_acertos));

            RuleFor(x => x.transacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'transacao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.qtde_total)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_total));

            RuleFor(x => x.id_remessa_operacoes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessa_operacoes));

            RuleFor(x => x.id_remessas_itens)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas_itens));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
