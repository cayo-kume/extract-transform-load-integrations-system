using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxMovimentoRemessasAcertosValidator : AbstractValidator<LinxMovimentoRemessasAcertos>
    {
        public LinxMovimentoRemessasAcertosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_remessas_acertos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_remessas_acertos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_remessas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_remessas));

            RuleFor(x => x.identificador_venda)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_venda));

            RuleFor(x => x.identificador_retorno)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_retorno));

            RuleFor(x => x.identificador_devolucao)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_devolucao));

            RuleFor(x => x.data)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data));

            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
