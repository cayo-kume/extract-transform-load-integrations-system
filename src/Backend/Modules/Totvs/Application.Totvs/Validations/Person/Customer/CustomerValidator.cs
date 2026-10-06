using Application.Totvs.Validations.Person.LegalEntity;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Person.Customer;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class CustomerValidator : AbstractValidator<Item>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.cpf)
                .MaximumLength(14)
                .WithMessage("O campo 'cpf' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpf));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleFor(x => x.maritalStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'maritalStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.maritalStatus));

            RuleFor(x => x.gender)
                .MaximumLength(60)
                .WithMessage("O campo 'gender' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.gender));

            RuleFor(x => x.ctps)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ctps));

            RuleFor(x => x.ctpsSerial)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ctpsSerial));

            RuleFor(x => x.rg)
                .MaximumLength(11)
                .WithMessage("O campo 'rg' deve ter no máximo 11 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rg));

            RuleFor(x => x.rgFederalAgency)
                .MaximumLength(60)
                .WithMessage("O campo 'rgFederalAgency' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rgFederalAgency));

            RuleFor(x => x.suframaCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.suframaCode));

            RuleFor(x => x.motherName)
                .MaximumLength(250)
                .WithMessage("O campo 'motherName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motherName));

            RuleFor(x => x.fatherName)
                .MaximumLength(250)
                .WithMessage("O campo 'fatherName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fatherName));

            RuleFor(x => x.nationality)
                .MaximumLength(60)
                .WithMessage("O campo 'nationality' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nationality));

            RuleFor(x => x.birthDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.birthDate));

            RuleFor(x => x.monthlyIncome)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.monthlyIncome));

            RuleFor(x => x.occupation)
                .MaximumLength(60)
                .WithMessage("O campo 'occupation' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.occupation));

            RuleFor(x => x.hireDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.hireDate));

            RuleFor(x => x.workPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'workPlace' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.workPlace));

            RuleFor(x => x.homeTown)
                .MaximumLength(60)
                .WithMessage("O campo 'homeTown' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.homeTown));

            RuleFor(x => x.branchInsertCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchInsertCode));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.homePage)
                .MaximumLength(300)
                .WithMessage("O campo 'homePage' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.homePage));

            RuleFor(x => x.codeMainRelated)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codeMainRelated));

            RuleFor(x => x.cpfCnpjMainRelated)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpjMainRelated' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpjMainRelated));

            RuleFor(x => x.nameMainRelated)
                .MaximumLength(250)
                .WithMessage("O campo 'nameMainRelated' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nameMainRelated));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.isCustomer)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isCustomer));

            RuleFor(x => x.isSupplier)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isSupplier));

            RuleFor(x => x.isRepresentative)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isRepresentative));

            RuleFor(x => x.isPurchasingGuide)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isPurchasingGuide));

            RuleFor(x => x.isShippingCompany)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isShippingCompany));

            RuleFor(x => x.isEmployee)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isEmployee));

            RuleFor(x => x.employeeStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'employeeStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.employeeStatus));

            RuleFor(x => x.customerStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'customerStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerStatus));

            RuleFor(x => x.preferences)
                .SetValidator(new PreferencesValidator());

            RuleFor(x => x.shippingCompany)
                .SetValidator(new ShippingCompanyValidator());

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());

            RuleForEach(x => x.familiars)
                .SetValidator(new FamiliarValidator());

            RuleForEach(x => x.phones)
                .SetValidator(new PhoneValidator());

            RuleForEach(x => x.emails)
                .SetValidator(new EmailValidator());

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationValidator());

            RuleForEach(x => x.customerObservations)
                .SetValidator(new CustomerObservationValidator());

            RuleForEach(x => x.additionalFields)
                .SetValidator(new AdditionalfieldValidator());

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationValidator());

            RuleForEach(x => x.references)
                .SetValidator(new ReferenceValidator());

            RuleForEach(x => x.relateds)
                .SetValidator(new RelatedValidator());

            RuleForEach(x => x.statistics)
                .SetValidator(new StatisticValidator());

            RuleForEach(x => x.representatives)
                .SetValidator(new RepresentativeValidator());
        }
    }
}
