using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.FiscalDocuments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script058_Create_LinxMicrovix_Fiscal_Documents_Tables
    /// </summary>
    public class LinxXMLDocumentosValidator : AbstractValidator<LinxXMLDocumentos>
    {
        public LinxXMLDocumentosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.documento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.serie)
                .MaximumLength(10)
                .WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie));

            RuleFor(x => x.data_emissao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_emissao));

            RuleFor(x => x.chave_nfe)
                .MaximumLength(44)
                .WithMessage("O campo 'chave_nfe' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave_nfe));

            RuleFor(x => x.situacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.situacao));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.identificador_microvix)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'identificador_microvix' é obrigatório.")
                .MustBeValidGuid();

            RuleFor(x => x.dt_insert)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insert));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.nProtCanc)
                .MaximumLength(20)
                .WithMessage("O campo 'nProtCanc' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nProtCanc));

            RuleFor(x => x.nProtInut)
                .MaximumLength(20)
                .WithMessage("O campo 'nProtInut' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nProtInut));

            RuleFor(x => x.nProtDeneg)
                .MaximumLength(15)
                .WithMessage("O campo 'nProtDeneg' deve ter no máximo 15 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nProtDeneg));

            RuleFor(x => x.cStat)
                .MaximumLength(5)
                .WithMessage("O campo 'cStat' deve ter no máximo 5 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cStat));

            RuleFor(x => x.id_nfe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe));

            RuleFor(x => x.justificativa)
                .MaximumLength(255)
                .WithMessage("O campo 'justificativa' deve ter no máximo 255 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.justificativa));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));
        }
    }
}
