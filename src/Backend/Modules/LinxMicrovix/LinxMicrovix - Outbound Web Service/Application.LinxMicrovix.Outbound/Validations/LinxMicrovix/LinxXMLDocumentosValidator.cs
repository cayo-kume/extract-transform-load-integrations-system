using Domain.Core.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Validations.LinxMicrovix
{
    public class LinxXMLDocumentosValidator : AbstractValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxXMLDocumentos>
    {
        public LinxXMLDocumentosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.documento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.documento));
            RuleFor(x => x.serie).MaximumLength(10).WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie));
            RuleFor(x => x.data_emissao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_emissao));
            RuleFor(x => x.chave_nfe).MaximumLength(44).WithMessage("O campo 'chave_nfe' deve ter no máximo 44 caracteres.").When(x => !string.IsNullOrEmpty(x.chave_nfe));
            RuleFor(x => x.situacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.situacao));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.identificador_microvix).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.identificador_microvix));
            RuleFor(x => x.dt_insert).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_insert));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.nProtCanc).MaximumLength(15).WithMessage("O campo 'nProtCanc' deve ter no máximo 15 caracteres.").When(x => !string.IsNullOrEmpty(x.nProtCanc));
            RuleFor(x => x.nProtInut).MaximumLength(15).WithMessage("O campo 'nProtInut' deve ter no máximo 15 caracteres.").When(x => !string.IsNullOrEmpty(x.nProtInut));
            RuleFor(x => x.nProtDeneg).MaximumLength(15).WithMessage("O campo 'nProtDeneg' deve ter no máximo 15 caracteres.").When(x => !string.IsNullOrEmpty(x.nProtDeneg));
            RuleFor(x => x.cStat).MaximumLength(5).WithMessage("O campo 'cStat' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.cStat));
            RuleFor(x => x.id_nfe).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_nfe));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
        }
    }
}
