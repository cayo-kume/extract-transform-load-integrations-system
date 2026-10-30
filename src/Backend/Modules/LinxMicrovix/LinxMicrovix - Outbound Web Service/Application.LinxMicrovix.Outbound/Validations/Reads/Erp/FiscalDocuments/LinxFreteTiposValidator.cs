using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxFreteTiposValidator : AbstractValidator<LinxFreteTipos>
    {
        public LinxFreteTiposValidator()
        {
            RuleFor(x => x.id_frete_tipos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_frete_tipos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(40)
                .WithMessage("O campo 'descricao' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.codigo_sped)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_sped));

            RuleFor(x => x.codigo_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_nfe));

            RuleFor(x => x.codigo_sintegra)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_sintegra));

            RuleFor(x => x.permite_valor)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.permite_valor));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
