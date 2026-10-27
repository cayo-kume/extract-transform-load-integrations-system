using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxNfseValidator : AbstractValidator<LinxNfse>
    {
        public LinxNfseValidator()
        {
            RuleFor(x => x.portal)
                .MaximumLength(20)
                .WithMessage("O campo 'portal' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.documento)
                .MaximumLength(20)
                .WithMessage("O campo 'documento' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.id_nfse)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_nfse' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo_verificacao)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_verificacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_verificacao));

            RuleFor(x => x.id_nfse_situacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfse_situacao));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.dt_insert)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insert));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
