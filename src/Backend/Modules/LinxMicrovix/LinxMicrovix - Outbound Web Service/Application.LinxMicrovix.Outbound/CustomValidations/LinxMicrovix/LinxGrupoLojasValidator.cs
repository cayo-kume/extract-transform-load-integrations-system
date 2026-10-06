using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxGrupoLojasValidator : AbstractValidator<LinxGrupoLojas>
    {
        public LinxGrupoLojasValidator()
        {
            RuleFor(x => x.cnpj).MaximumLength(14).WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj));
            RuleFor(x => x.nome_empresa).MaximumLength(50).WithMessage("O campo 'nome_empresa' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_empresa));
            RuleFor(x => x.id_empresas_rede).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_empresas_rede));
            RuleFor(x => x.rede).MaximumLength(100).WithMessage("O campo 'rede' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.rede));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.nome_portal).MaximumLength(50).WithMessage("O campo 'nome_portal' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.classificacao_portal).MaximumLength(50).WithMessage("O campo 'classificacao_portal' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.classificacao_portal));
        }
    }
}