using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxCstIpiFiscalValidator : AbstractValidator<LinxCstIpiFiscal>
    {
        public LinxCstIpiFiscalValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_cst_ipi_fiscal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_cst_ipi_fiscal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cst_ipi_fiscal)
                .MaximumLength(4)
                .WithMessage("O campo 'cst_ipi_fiscal' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cst_ipi_fiscal));

            RuleFor(x => x.descricao)
                .MaximumLength(150)
                .WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.inicio_vigencia)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.inicio_vigencia));

            RuleFor(x => x.termino_vigencia)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.termino_vigencia));

            RuleFor(x => x.cst_ipi_fiscal_inverso)
                .MaximumLength(4)
                .WithMessage("O campo 'cst_ipi_fiscal_inverso' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cst_ipi_fiscal_inverso));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
