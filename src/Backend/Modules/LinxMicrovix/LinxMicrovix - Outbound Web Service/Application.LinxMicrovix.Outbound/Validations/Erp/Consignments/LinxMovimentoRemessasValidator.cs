using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxMovimentoRemessasValidator : AbstractValidator<LinxMovimentoRemessas>
    {
        public LinxMovimentoRemessasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_remessas)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_remessas' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_tipo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tipo));

            RuleFor(x => x.identificador_remessa)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_remessa));

            RuleFor(x => x.status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.status_descricao)
                .MaximumLength(35)
                .WithMessage("O campo 'status_descricao' deve ter no máximo 35 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status_descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
