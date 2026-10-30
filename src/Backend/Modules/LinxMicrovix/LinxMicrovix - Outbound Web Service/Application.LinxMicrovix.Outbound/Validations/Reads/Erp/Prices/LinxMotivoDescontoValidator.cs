using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxMotivoDescontoValidator : AbstractValidator<LinxMotivoDesconto>
    {
        public LinxMotivoDescontoValidator()
        {
            RuleFor(x => x.id_motivo_desconto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_motivo_desconto' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.desc_motivo_desconto)
                .MaximumLength(60)
                .WithMessage("O campo 'desc_motivo_desconto' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_motivo_desconto));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
