using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPlanoContasValidator : AbstractValidator<LinxPlanoContas>
    {
        public LinxPlanoContasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.cnpj).MaximumLength(14).WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj));
            RuleFor(x => x.id_conta).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_conta));
            RuleFor(x => x.nome_conta).MaximumLength(50).WithMessage("O campo 'nome_conta' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_conta));
            RuleFor(x => x.sintetica).MaximumLength(1).WithMessage("O campo 'sintetica' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sintetica));
            RuleFor(x => x.indice).MaximumLength(150).WithMessage("O campo 'indice' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.indice));
            RuleFor(x => x.ativa).MaximumLength(1).WithMessage("O campo 'ativa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.ativa));
            RuleFor(x => x.fluxo_caixa).MaximumLength(1).WithMessage("O campo 'fluxo_caixa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.fluxo_caixa));
            RuleFor(x => x.conta_contabil).MaximumLength(20).WithMessage("O campo 'conta_contabil' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.conta_contabil));
            RuleFor(x => x.id_natureza_conta).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_natureza_conta));
            RuleFor(x => x.conta_bancaria).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.conta_bancaria));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}