using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxRotinaOrigemValidator : AbstractValidator<LinxRotinaOrigem>
    {
        public LinxRotinaOrigemValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.codigo_rotina)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_rotina' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_rotina)
                .MaximumLength(150)
                .WithMessage("O campo 'descricao_rotina' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_rotina));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
