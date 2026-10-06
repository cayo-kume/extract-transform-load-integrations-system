using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesFornecCamposAdicionaisValidator : AbstractValidator<LinxClientesFornecCamposAdicionais>
    {
        public LinxClientesFornecCamposAdicionaisValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.campo).MaximumLength(50).WithMessage("O campo 'campo' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.campo));
            RuleFor(x => x.valor).MaximumLength(100).WithMessage("O campo 'valor' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.valor));
        }
    }
}
