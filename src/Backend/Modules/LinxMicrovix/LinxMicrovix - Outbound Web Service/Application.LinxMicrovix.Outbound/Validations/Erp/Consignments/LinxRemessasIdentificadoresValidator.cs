using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxRemessasIdentificadoresValidator : AbstractValidator<LinxRemessasIdentificadores>
    {
        public LinxRemessasIdentificadoresValidator()
        {
            RuleFor(x => x.identificador_venda)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_venda));

            RuleFor(x => x.identificador_remessa)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_remessa));

            RuleFor(x => x.id_remessas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas));

            RuleFor(x => x.id_remessas_acertos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas_acertos));

            RuleFor(x => x.transacao_acerto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_acerto));

            RuleFor(x => x.qtde_total_acerto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_total_acerto));

            RuleFor(x => x.transacao_remessa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_remessa));

            RuleFor(x => x.identificador_devolucao)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_devolucao));

            RuleFor(x => x.id_remessa_operacoes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessa_operacoes));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
