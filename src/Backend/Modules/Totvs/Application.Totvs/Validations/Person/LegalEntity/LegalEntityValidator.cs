using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class LegalEntityValidator : AbstractValidator<Item>
    {
        public LegalEntityValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.cnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleFor(x => x.branchInsertCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchInsertCode));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.fantasyName)
                .MaximumLength(250)
                .WithMessage("O campo 'fantasyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fantasyName));

            RuleFor(x => x.uf)
                .MaximumLength(20)
                .WithMessage("O campo 'uf' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.uf));

            RuleFor(x => x.numberStateRegistration)
                .MaximumLength(60)
                .WithMessage("O campo 'numberStateRegistration' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numberStateRegistration));

            RuleFor(x => x.suframaCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.suframaCode));

            RuleFor(x => x.dateFoundation)
                .MaximumLength(60)
                .WithMessage("O campo 'dateFoundation' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.dateFoundation));

            RuleFor(x => x.codeActivity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codeActivity));

            RuleFor(x => x.numberEmployees)
                .MaximumLength(60)
                .WithMessage("O campo 'numberEmployees' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numberEmployees));

            RuleFor(x => x.monthlyInvoicing)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.monthlyInvoicing));

            RuleFor(x => x.shareCapital)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shareCapital));

            RuleFor(x => x.homePage)
                .MaximumLength(300)
                .WithMessage("O campo 'homePage' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.homePage));

            RuleFor(x => x.codeActivityCnae)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codeActivityCnae));

            RuleFor(x => x.codeActivityCnae2)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codeActivityCnae2));

            RuleFor(x => x.typeTaxRegime)
                .MaximumLength(60)
                .WithMessage("O campo 'typeTaxRegime' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeTaxRegime));

            RuleFor(x => x.typeSubTributary)
                .MaximumLength(60)
                .WithMessage("O campo 'typeSubTributary' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeSubTributary));

            RuleFor(x => x.typeDescriptionSuFrama)
                .MaximumLength(60)
                .WithMessage("O campo 'typeDescriptionSuFrama' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeDescriptionSuFrama));

            RuleFor(x => x.registrationMunicipal)
                .MaximumLength(60)
                .WithMessage("O campo 'registrationMunicipal' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.registrationMunicipal));

            RuleFor(x => x.descriptionJuntaCial)
                .MaximumLength(60)
                .WithMessage("O campo 'descriptionJuntaCial' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descriptionJuntaCial));

            RuleFor(x => x.dateRegJuntaCial)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dateRegJuntaCial));

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

            RuleFor(x => x.customerStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'customerStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerStatus));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.preferences)
                .SetValidator(new PreferencesValidator());

            RuleFor(x => x.shippingCompany)
                .SetValidator(new ShippingCompanyValidator());

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());

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

            RuleForEach(x => x.partners)
                .SetValidator(new PartnerValidator());

            RuleForEach(x => x.contacts)
                .SetValidator(new ContactValidator());

            RuleForEach(x => x.statistics)
                .SetValidator(new StatisticValidator());

            RuleForEach(x => x.socialNetworks)
                .SetValidator(new SocialNetworkValidator());

            RuleForEach(x => x.representatives)
                .SetValidator(new RepresentativeValidator());
        }
    }
}
