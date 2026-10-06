using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Customer;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Customer
{
    public class PersonAddressValidator : AbstractValidator<PersonAddress>
    {
        public PersonAddressValidator()
        {
            RuleFor(x => x.Name)
                .TruncateIfExceeds(60, (x, val) => x.Name = val);

            RuleFor(x => x.ContactName)
                .TruncateIfExceeds(60, (x, val) => x.ContactName = val);

            RuleFor(x => x.PostalCode)
                .TruncateIfExceeds(9, (x, val) => x.PostalCode = val);

            RuleFor(x => x.AddressLine)
                .TruncateIfExceeds(250, (x, val) => x.AddressLine = val);

            RuleFor(x => x.City)
                .TruncateIfExceeds(40, (x, val) => x.City = val);

            RuleFor(x => x.Neighbourhood)
                .TruncateIfExceeds(60, (x, val) => x.Neighbourhood = val);

            RuleFor(x => x.Number)
                .TruncateIfExceeds(20, (x, val) => x.Number = val);

            RuleFor(x => x.State)
                .TruncateIfExceeds(20, (x, val) => x.State = val);

            RuleFor(x => x.AddressNotes)
                .TruncateIfExceeds(60, (x, val) => x.AddressNotes = val);

            RuleFor(x => x.Landmark)
                .TruncateIfExceeds(60, (x, val) => x.Landmark = val);
        }
    }
}
