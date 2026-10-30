using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Consignments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script049_Create_LinxMicrovix_Consignments_Tables
    /// </summary>
    public class LinxRemessasOperacoesValidator : AbstractValidator<LinxRemessasOperacoes>
    {
        public LinxRemessasOperacoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_remessa_operacoes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_remessa_operacoes' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(30)
                .WithMessage("O campo 'descricao' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
