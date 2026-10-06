using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Customer;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Customer
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.Phone)
                .TruncateIfExceeds(20, (x, val) => x.Phone = val);

            RuleFor(x => x.Phone2)
                .TruncateIfExceeds(20, (x, val) => x.Phone2 = val);

            RuleFor(x => x.CellPhone)
                .TruncateIfExceeds(20, (x, val) => x.CellPhone = val);

            RuleFor(x => x.Fax)
                .TruncateIfExceeds(10, (x, val) => x.Fax = val);
        }
    }
}
