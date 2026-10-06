using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMetodosValidator : AbstractValidator<LinxMetodos>
    {
        public LinxMetodosValidator()
        {
            RuleFor(x => x.methodID).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.methodID));
        }
    }
}