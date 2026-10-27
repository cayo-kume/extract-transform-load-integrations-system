using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxClasseFiscalValidator : AbstractValidator<LinxClasseFiscal>
    {
        public LinxClasseFiscalValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_classe_fiscal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_classe_fiscal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(150)
                .WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
