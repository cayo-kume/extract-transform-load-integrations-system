using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxNFeEventoValidator : AbstractValidator<LinxNFeEvento>
    {
        public LinxNFeEventoValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.id_nfe_evento)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_nfe_evento' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe));

            RuleFor(x => x.codigo_tipo)
                .MaximumLength(6)
                .WithMessage("O campo 'codigo_tipo' deve ter no máximo 6 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_tipo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.data_emissao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_emissao));

            RuleFor(x => x.hora_emissao)
                .MaximumLength(20)
                .WithMessage("O campo 'hora_emissao' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.hora_emissao));
        }
    }
}
