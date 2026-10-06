using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoCamposAdicionaisValidator : AbstractValidator<LinxMovimentoCamposAdicionais>
    {
        public LinxMovimentoCamposAdicionaisValidator()
        {
            RuleFor(x => x.id_ordservprod).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ordservprod));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.paciente).MaximumLength(100).WithMessage("O campo 'paciente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.paciente));
            RuleFor(x => x.codigo_gerencial).MaximumLength(40).WithMessage("O campo 'codigo_gerencial' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_gerencial));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
