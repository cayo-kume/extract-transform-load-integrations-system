using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxEntradaNFeValidator : AbstractValidator<LinxEntradaNFe>
    {
        public LinxEntradaNFeValidator()
        {
            RuleFor(x => x.id_entrada_nfe)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_entrada_nfe' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.numero_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_nfe));

            RuleFor(x => x.lancada_no_microvix)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.lancada_no_microvix));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.documento_emitente)
                .MaximumLength(14)
                .WithMessage("O campo 'documento_emitente' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documento_emitente));

            RuleFor(x => x.razao_social_emitente)
                .MaximumLength(60)
                .WithMessage("O campo 'razao_social_emitente' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.razao_social_emitente));

            RuleFor(x => x.serie_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.serie_nfe));

            RuleFor(x => x.data_emissao_nfe)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_emissao_nfe));

            RuleFor(x => x.chave_nfe)
                .MaximumLength(44)
                .WithMessage("O campo 'chave_nfe' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave_nfe));

            RuleFor(x => x.excluida)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluida));

            RuleFor(x => x.dt_insert)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insert));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
