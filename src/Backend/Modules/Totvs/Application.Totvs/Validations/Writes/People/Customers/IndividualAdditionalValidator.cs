using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// dados adicionais da pessoa física (IndividualAdditionalInDto): os documentos vão mascarados para totvs.TotvsRequests (T6): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class IndividualAdditionalValidator : AbstractValidator<IndividualAdditional>
    {
        public IndividualAdditionalValidator()
        {
            RuleFor(x => x.educationLevel)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.EducationLevel))
                .WithMessage($"O campo 'educationLevel' só aceita {CustomerRules.Names(CustomerEnumNames.EducationLevel)}.");

            RuleFor(x => x.numberOfChildren)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'numberOfChildren' deve ter até 2 dígitos (0 zera).")
                .When(x => x.numberOfChildren is not null);

            RuleFor(x => x.numberOfDependents)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'numberOfDependents' deve ter até 2 dígitos (0 zera).")
                .When(x => x.numberOfDependents is not null);

            RuleFor(x => x.residenceMonths)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'residenceMonths' deve ter até 4 dígitos (0 zera).")
                .When(x => x.residenceMonths is not null);

            RuleFor(x => x.workMonths)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'workMonths' deve ter até 4 dígitos (0 zera).")
                .When(x => x.workMonths is not null);

            RuleFor(x => x.previousWorkPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'previousWorkPlace' aceita até 60 caracteres.");

            RuleFor(x => x.residingSinceDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'residingSinceDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.homeType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Home))
                .WithMessage($"O campo 'homeType' só aceita {CustomerRules.Names(CustomerEnumNames.Home)}.");

            RuleFor(x => x.carType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Car))
                .WithMessage($"O campo 'carType' só aceita {CustomerRules.Names(CustomerEnumNames.Car)}.");

            RuleFor(x => x.score)
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'score' aceita até 9 inteiros (0 zera).")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'score' aceita até 3 casas decimais.")
                .When(x => x.score is not null);

            RuleFor(x => x.risk)
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'risk' aceita até 9 inteiros (0 zera).")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'risk' aceita até 3 casas decimais.")
                .When(x => x.risk is not null);

            RuleFor(x => x.spcUpdateDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'spcUpdateDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.streetVendorNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'streetVendorNumber' aceita até 20 caracteres.");

            RuleFor(x => x.rgType)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'rgType' deve ter até 2 dígitos (0 zera).")
                .When(x => x.rgType is not null);

            RuleFor(x => x.rgIssueDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'rgIssueDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.passportNumber)
                .MaximumLength(40)
                .WithMessage("O campo 'passportNumber' aceita até 40 caracteres.");

            RuleFor(x => x.passportCountryCode)
                .InclusiveBetween(0, 99_999)
                .WithMessage("O campo 'passportCountryCode' deve ter até 5 dígitos (0 zera).")
                .When(x => x.passportCountryCode is not null);

            RuleFor(x => x.driverLicenseNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseNumber' aceita até 20 caracteres.");

            RuleFor(x => x.driverLicenseExpirationDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'driverLicenseExpirationDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.firstDriverLicenseDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'firstDriverLicenseDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.driverLicenseAgency)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseAgency' aceita até 20 caracteres.");

            RuleFor(x => x.driverLicenseCategory)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.DriverLicenseCategory))
                .WithMessage($"O campo 'driverLicenseCategory' só aceita {CustomerRules.Names(CustomerEnumNames.DriverLicenseCategory)}.");

            RuleFor(x => x.voterRegistrationNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'voterRegistrationNumber' aceita até 20 caracteres.");

            RuleFor(x => x.voterZone)
                .MaximumLength(10)
                .WithMessage("O campo 'voterZone' aceita até 10 caracteres.");

            RuleFor(x => x.voterSection)
                .MaximumLength(10)
                .WithMessage("O campo 'voterSection' aceita até 10 caracteres.");

            RuleFor(x => x.militaryCertificateType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.MilitaryCertificate))
                .WithMessage($"O campo 'militaryCertificateType' só aceita {CustomerRules.Names(CustomerEnumNames.MilitaryCertificate)}.");

            RuleFor(x => x.militaryCategory)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.MilitaryCategory))
                .WithMessage($"O campo 'militaryCategory' só aceita {CustomerRules.Names(CustomerEnumNames.MilitaryCategory)}.");

            RuleFor(x => x.militaryDispensation)
                .MaximumLength(20)
                .WithMessage("O campo 'militaryDispensation' aceita até 20 caracteres.");

            RuleFor(x => x.nitNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'nitNumber' aceita até 20 caracteres.");

            RuleFor(x => x.ctpsState)
                .MaximumLength(2)
                .WithMessage("O campo 'ctpsState' aceita até 2 caracteres.");

            RuleFor(x => x.pisNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'pisNumber' aceita até 20 caracteres.");

            RuleFor(x => x.pisIssueDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'pisIssueDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.pisState)
                .MaximumLength(2)
                .WithMessage("O campo 'pisState' aceita até 2 caracteres.");

            RuleFor(x => x.ruralProducerRegistration)
                .MaximumLength(18)
                .WithMessage("O campo 'ruralProducerRegistration' aceita até 18 caracteres.");

            RuleFor(x => x.ruralProducerState)
                .MaximumLength(2)
                .WithMessage("O campo 'ruralProducerState' aceita até 2 caracteres.");

            RuleFor(x => x.cifIssuer)
                .MaximumLength(20)
                .WithMessage("O campo 'cifIssuer' aceita até 20 caracteres.");

            RuleFor(x => x.cifState)
                .MaximumLength(2)
                .WithMessage("O campo 'cifState' aceita até 2 caracteres.");

            RuleFor(x => x.cifNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'cifNumber' aceita até 20 caracteres.");

            RuleFor(x => x.birthState)
                .MaximumLength(2)
                .WithMessage("O campo 'birthState' aceita até 2 caracteres.");

            RuleFor(x => x.passportIssuer)
                .MaximumLength(10)
                .WithMessage("O campo 'passportIssuer' aceita até 10 caracteres.");

            RuleFor(x => x.passportState)
                .MaximumLength(2)
                .WithMessage("O campo 'passportState' aceita até 2 caracteres.");

            RuleFor(x => x.passportExpirationDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'passportExpirationDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.ricNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'ricNumber' aceita até 20 caracteres.");

            RuleFor(x => x.ricState)
                .MaximumLength(2)
                .WithMessage("O campo 'ricState' aceita até 2 caracteres.");

            RuleFor(x => x.ricIssuer)
                .MaximumLength(20)
                .WithMessage("O campo 'ricIssuer' aceita até 20 caracteres.");

            RuleFor(x => x.ricCityCode)
                .InclusiveBetween(0, 999_999)
                .WithMessage("O campo 'ricCityCode' deve ter até 6 dígitos (0 zera).")
                .When(x => x.ricCityCode is not null);

            RuleFor(x => x.ricIssueDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'ricIssueDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.ricExpirationDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'ricExpirationDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.foreignDocumentNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'foreignDocumentNumber' aceita até 20 caracteres.");

            RuleFor(x => x.nickname)
                .MaximumLength(40)
                .WithMessage("O campo 'nickname' aceita até 40 caracteres.");

            RuleFor(x => x.crcNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'crcNumber' aceita até 20 caracteres.");

            RuleFor(x => x.driverLicenseSecurityNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseSecurityNumber' aceita até 20 caracteres.");

            RuleFor(x => x.driverLicenseRegistrationNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseRegistrationNumber' aceita até 20 caracteres.");

            RuleFor(x => x.crcRegistrationDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'crcRegistrationDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.crcExpirationDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'crcExpirationDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.socialName)
                .MaximumLength(70)
                .WithMessage("O campo 'socialName' aceita até 70 caracteres.");

            RuleFor(x => x.homeTown)
                .InclusiveBetween(0, 999_999)
                .WithMessage("O campo 'homeTown' deve ter até 6 dígitos (0 zera).")
                .When(x => x.homeTown is not null);
        }
    }
}
