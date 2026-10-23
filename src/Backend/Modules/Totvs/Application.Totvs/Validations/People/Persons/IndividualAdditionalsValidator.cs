using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script013_Create_Totvs_People_Tables)
    /// </summary>
    public class IndividualAdditionalsValidator : AbstractValidator<IndividualAdditionals>
    {
        public IndividualAdditionalsValidator()
        {
            RuleFor(x => x.educationLevel)
                .MaximumLength(40)
                .WithMessage("O campo 'educationLevel' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.educationLevel));

            RuleFor(x => x.numberOfChildren)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numberOfChildren));

            RuleFor(x => x.numberOfDependents)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numberOfDependents));

            RuleFor(x => x.residenceMonths)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.residenceMonths));

            RuleFor(x => x.workMonths)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.workMonths));

            RuleFor(x => x.previousWorkPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'previousWorkPlace' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.previousWorkPlace));

            RuleFor(x => x.residingSinceDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.residingSinceDate));

            RuleFor(x => x.homeType)
                .MaximumLength(40)
                .WithMessage("O campo 'homeType' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.homeType));

            RuleFor(x => x.carType)
                .MaximumLength(40)
                .WithMessage("O campo 'carType' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.carType));

            RuleFor(x => x.score)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.score));

            RuleFor(x => x.risk)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.risk));

            RuleFor(x => x.spcUpdateDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.spcUpdateDate));

            RuleFor(x => x.isStreetVendor)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isStreetVendor));

            RuleFor(x => x.streetVendorNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'streetVendorNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.streetVendorNumber));

            RuleFor(x => x.rgType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.rgType));

            RuleFor(x => x.rgIssueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.rgIssueDate));

            RuleFor(x => x.passportNumber)
                .MaximumLength(40)
                .WithMessage("O campo 'passportNumber' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.passportNumber));

            RuleFor(x => x.passportCountryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.passportCountryCode));

            RuleFor(x => x.driverLicenseNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.driverLicenseNumber));

            RuleFor(x => x.driverLicenseExpirationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.driverLicenseExpirationDate));

            RuleFor(x => x.firstDriverLicenseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.firstDriverLicenseDate));

            RuleFor(x => x.driverLicenseAgency)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseAgency' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.driverLicenseAgency));

            RuleFor(x => x.driverLicenseCategory)
                .MaximumLength(40)
                .WithMessage("O campo 'driverLicenseCategory' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.driverLicenseCategory));

            RuleFor(x => x.voterRegistrationNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'voterRegistrationNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.voterRegistrationNumber));

            RuleFor(x => x.voterZone)
                .MaximumLength(10)
                .WithMessage("O campo 'voterZone' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.voterZone));

            RuleFor(x => x.voterSection)
                .MaximumLength(10)
                .WithMessage("O campo 'voterSection' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.voterSection));

            RuleFor(x => x.militaryCertificateType)
                .MaximumLength(40)
                .WithMessage("O campo 'militaryCertificateType' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.militaryCertificateType));

            RuleFor(x => x.militaryCategory)
                .MaximumLength(40)
                .WithMessage("O campo 'militaryCategory' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.militaryCategory));

            RuleFor(x => x.militaryDispensation)
                .MaximumLength(20)
                .WithMessage("O campo 'militaryDispensation' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.militaryDispensation));

            RuleFor(x => x.nitNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'nitNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nitNumber));

            RuleFor(x => x.ctpsState)
                .MaximumLength(2)
                .WithMessage("O campo 'ctpsState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ctpsState));

            RuleFor(x => x.isInformalWorker)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInformalWorker));

            RuleFor(x => x.pisNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'pisNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.pisNumber));

            RuleFor(x => x.pisIssueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.pisIssueDate));

            RuleFor(x => x.pisState)
                .MaximumLength(2)
                .WithMessage("O campo 'pisState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.pisState));

            RuleFor(x => x.ruralProducerRegistration)
                .MaximumLength(18)
                .WithMessage("O campo 'ruralProducerRegistration' deve ter no máximo 18 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ruralProducerRegistration));

            RuleFor(x => x.ruralProducerState)
                .MaximumLength(2)
                .WithMessage("O campo 'ruralProducerState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ruralProducerState));

            RuleFor(x => x.cifIssuer)
                .MaximumLength(20)
                .WithMessage("O campo 'cifIssuer' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cifIssuer));

            RuleFor(x => x.cifState)
                .MaximumLength(2)
                .WithMessage("O campo 'cifState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cifState));

            RuleFor(x => x.cifNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'cifNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cifNumber));

            RuleFor(x => x.birthState)
                .MaximumLength(2)
                .WithMessage("O campo 'birthState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.birthState));

            RuleFor(x => x.passportIssuer)
                .MaximumLength(10)
                .WithMessage("O campo 'passportIssuer' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.passportIssuer));

            RuleFor(x => x.passportState)
                .MaximumLength(2)
                .WithMessage("O campo 'passportState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.passportState));

            RuleFor(x => x.passportIssueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.passportIssueDate));

            RuleFor(x => x.passportExpirationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.passportExpirationDate));

            RuleFor(x => x.ricNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'ricNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ricNumber));

            RuleFor(x => x.ricState)
                .MaximumLength(2)
                .WithMessage("O campo 'ricState' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ricState));

            RuleFor(x => x.ricIssuer)
                .MaximumLength(20)
                .WithMessage("O campo 'ricIssuer' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ricIssuer));

            RuleFor(x => x.ricCityCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ricCityCode));

            RuleFor(x => x.ricIssueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.ricIssueDate));

            RuleFor(x => x.ricExpirationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.ricExpirationDate));

            RuleFor(x => x.foreignDocumentNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'foreignDocumentNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.foreignDocumentNumber));

            RuleFor(x => x.nickname)
                .MaximumLength(40)
                .WithMessage("O campo 'nickname' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nickname));

            RuleFor(x => x.crcNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'crcNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.crcNumber));

            RuleFor(x => x.driverLicenseSecurityNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseSecurityNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.driverLicenseSecurityNumber));

            RuleFor(x => x.driverLicenseRegistrationNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'driverLicenseRegistrationNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.driverLicenseRegistrationNumber));

            RuleFor(x => x.crcRegistrationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.crcRegistrationDate));

            RuleFor(x => x.crcExpirationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.crcExpirationDate));

            RuleFor(x => x.socialName)
                .MaximumLength(70)
                .WithMessage("O campo 'socialName' deve ter no máximo 70 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.socialName));

            RuleFor(x => x.homeTown)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.homeTown));
        }
    }
}
