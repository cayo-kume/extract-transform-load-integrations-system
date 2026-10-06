using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Customer;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Customer
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Surname)
                .TruncateIfExceeds(60, (x, val) => x.Surname = val);

            RuleFor(x => x.Gender)
                .TruncateIfExceeds(1, (x, val) => x.Gender = val);

            RuleFor(x => x.RG)
                .TruncateIfExceeds(14, (x, val) => x.RG = val);

            RuleFor(x => x.Cpf)
                .TruncateIfExceeds(14, (x, val) => x.Cpf = val);

            RuleFor(x => x.WebSiteID)
                .TruncateIfExceeds(60, (x, val) => x.WebSiteID = val);

            RuleFor(x => x.Name)
                .TruncateIfExceeds(60, (x, val) => x.Name = val);

            RuleFor(x => x.Email)
                .TruncateIfExceeds(60, (x, val) => x.Email = val);

            RuleFor(x => x.Password)
                .TruncateIfExceeds(60, (x, val) => x.Password = val);

            RuleFor(x => x.Cnpj)
                .TruncateIfExceeds(14, (x, val) => x.Cnpj = val);

            RuleFor(x => x.TradingName)
                .TruncateIfExceeds(60, (x, val) => x.TradingName = val);

            RuleFor(x => x.CustomerGroupID)
                .TruncateIfExceeds(60, (x, val) => x.CustomerGroupID = val);

            RuleForEach(x => x.Groups)
                .SetValidator(new GroupsValidator());

            RuleFor(x => x.Contact)
                .SetValidator(new ContactValidator());

            RuleForEach(x => x.Address)
                .SetValidator(new PersonAddressValidator());

            RuleFor(x => x.EmailConfirmation)
                .SetValidator(new EmailConfirmationValidator());

            RuleFor(x => x.BirthDate)
                .NormalizeDateIfInvalid((x, val) => x.BirthDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: BirthDate | Value: {x.BirthDate}, Error when trying to convert value: {x.BirthDate.ToString()} to DateTime");

            RuleFor(x => x.CreatedDate)
                .NormalizeDateIfInvalid((x, val) => x.CreatedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CreatedDate | Value: {x.CreatedDate}, Error when trying to convert value: {x.CreatedDate.ToString()} to DateTime");
        }
    }
}
