using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.People
{
    public class PersonValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.People.Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(64)
                .WithMessage("O campo 'id' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.codeReferenceAdditional)
                .MaximumLength(64)
                .WithMessage("O campo 'codeReferenceAdditional' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codeReferenceAdditional));

            RuleFor(x => x.isActive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isActive));

            RuleFor(x => x.personType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personType));

            RuleFor(x => x.profileType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.profileType));

            RuleFor(x => x.accessProfile)
                .MaximumLength(128)
                .WithMessage("O campo 'accessProfile' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessProfile));

            RuleFor(x => x.corporateName)
                .MaximumLength(128)
                .WithMessage("O campo 'corporateName' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.corporateName));

            RuleFor(x => x.businessName)
                .MaximumLength(128)
                .WithMessage("O campo 'businessName' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.businessName));

            RuleFor(x => x.businessBranch)
                .MaximumLength(128)
                .WithMessage("O campo 'businessBranch' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.businessBranch));

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.userName)
                .MaximumLength(64)
                .WithMessage("O campo 'userName' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.userName));

            RuleFor(x => x.role)
                .MaximumLength(128)
                .WithMessage("O campo 'role' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.role));

            RuleFor(x => x.bossId)
                .MaximumLength(64)
                .WithMessage("O campo 'bossId' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bossId));

            RuleFor(x => x.bossName)
                .MaximumLength(128)
                .WithMessage("O campo 'bossName' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bossName));

            RuleFor(x => x.classification)
                .MaximumLength(128)
                .WithMessage("O campo 'classification' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classification));

            RuleFor(x => x.cultureId)
                .MaximumLength(32)
                .WithMessage("O campo 'cultureId' deve ter no máximo 32 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cultureId));

            RuleFor(x => x.timeZoneId)
                .MaximumLength(64)
                .WithMessage("O campo 'timeZoneId' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.timeZoneId));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.createdBy)
                .MaximumLength(128)
                .WithMessage("O campo 'createdBy' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.createdBy));

            RuleFor(x => x.changedDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.changedDate));

            RuleFor(x => x.changedBy)
                .MaximumLength(128)
                .WithMessage("O campo 'changedBy' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.changedBy));

            RuleForEach(x => x.teams)
                .NotEmpty()
                .WithMessage("O campo 'team' não pode ser vazio.")
                .MaximumLength(128)
                .WithMessage("O campo 'team' deve ter no máximo 128 caracteres.");

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());

            RuleForEach(x => x.contacts)
                .SetValidator(new ContactValidator());

            RuleForEach(x => x.emails)
                .SetValidator(new EmailValidator());

            RuleForEach(x => x.relationships)
                .SetValidator(new RelationshipValidator());

            RuleForEach(x => x.customFieldValues)
                .SetValidator(new CustomFieldValueValidator());

            RuleForEach(x => x.atAssets)
                .SetValidator(new AssetValidator());
        }
    }
}
