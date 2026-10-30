using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxCsosnFiscalValidator : AbstractValidator<LinxCsosnFiscal>
    {
        public LinxCsosnFiscalValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_csosn_fiscal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_csosn_fiscal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.csosn_fiscal)
                .MaximumLength(5)
                .WithMessage("O campo 'csosn_fiscal' deve ter no máximo 5 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.csosn_fiscal));

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.id_csosn_fiscal_substitutiva)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_csosn_fiscal_substitutiva));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
