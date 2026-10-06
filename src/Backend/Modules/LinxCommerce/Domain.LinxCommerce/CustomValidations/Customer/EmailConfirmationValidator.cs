using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Customer;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Customer
{
    public class EmailConfirmationValidator : AbstractValidator<EmailConfirmation>
    {
        public EmailConfirmationValidator()
        {
            RuleFor(x => x.Status)
                .TruncateIfExceeds(1, (x, val) => x.Status = val);
        }
    }
}
