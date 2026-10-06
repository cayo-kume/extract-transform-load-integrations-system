using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesFornecContatosParentescoValidator : AbstractValidator<LinxClientesFornecContatosParentesco>
    {
        public LinxClientesFornecContatosParentescoValidator()
        {
            RuleFor(x => x.id_parentesco).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_parentesco));
            RuleFor(x => x.descricao_parentesco).MaximumLength(50).WithMessage("O campo 'descricao_parentesco' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_parentesco));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
