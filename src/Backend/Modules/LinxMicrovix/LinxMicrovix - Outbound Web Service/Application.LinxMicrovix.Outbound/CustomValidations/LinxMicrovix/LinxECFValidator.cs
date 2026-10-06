using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxECFValidator : AbstractValidator<LinxECF>
    {
        public LinxECFValidator()
        {
            RuleFor(x => x.id_ecf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ecf));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.ecf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ecf));
            RuleFor(x => x.numeroserie).MaximumLength(30).WithMessage("O campo 'numeroserie' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.numeroserie));
            RuleFor(x => x.indice_sangria).MaximumLength(4).WithMessage("O campo 'indice_sangria' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_sangria));
            RuleFor(x => x.modelo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.modelo));
            RuleFor(x => x.nome).MaximumLength(30).WithMessage("O campo 'nome' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.nome));
            RuleFor(x => x.indice_suprimento).MaximumLength(4).WithMessage("O campo 'indice_suprimento' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_suprimento));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.indice_sinal).MaximumLength(53).WithMessage("O campo 'indice_sinal' deve ter no máximo 53 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_sinal));
            RuleFor(x => x.indice_antecipacao).MaximumLength(50).WithMessage("O campo 'indice_antecipacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_antecipacao));
            RuleFor(x => x.indice_cartao_credito).MaximumLength(50).WithMessage("O campo 'indice_cartao_credito' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_cartao_credito));
            RuleFor(x => x.empresa_ecf).MaximumLength(50).WithMessage("O campo 'empresa_ecf' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.empresa_ecf));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
