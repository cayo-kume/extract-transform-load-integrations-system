using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxCfopFiscalValidator : AbstractValidator<LinxCfopFiscal>
    {
        public LinxCfopFiscalValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_cfop_fiscal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_cfop_fiscal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cfop_fiscal)
                .MaximumLength(5)
                .WithMessage("O campo 'cfop_fiscal' deve ter no máximo 5 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop_fiscal));

            RuleFor(x => x.descricao)
                .MaximumLength(300)
                .WithMessage("O campo 'descricao' deve ter no máximo 300 caracteres.")
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
