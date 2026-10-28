namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// dados adicionais da pessoa física (IndividualAdditionalInDto): os documentos vão mascarados para totvs.TotvsRequests (T6)
    /// </summary>
    public class IndividualAdditional
    {
        public string? educationLevel { get; init; }
        public int? numberOfChildren { get; init; }
        public int? numberOfDependents { get; init; }
        public int? residenceMonths { get; init; }
        public int? workMonths { get; init; }
        public string? previousWorkPlace { get; init; }
        public string? residingSinceDate { get; init; }
        public string? homeType { get; init; }
        public string? carType { get; init; }
        public decimal? score { get; init; }
        public decimal? risk { get; init; }
        public string? spcUpdateDate { get; init; }
        public bool? isStreetVendor { get; init; }
        public string? streetVendorNumber { get; init; }
        public int? rgType { get; init; }
        public string? rgIssueDate { get; init; }
        public string? passportNumber { get; init; }
        public int? passportCountryCode { get; init; }
        public string? driverLicenseNumber { get; init; }
        public string? driverLicenseExpirationDate { get; init; }
        public string? firstDriverLicenseDate { get; init; }
        public string? driverLicenseAgency { get; init; }
        public string? driverLicenseCategory { get; init; }
        public string? voterRegistrationNumber { get; init; }
        public string? voterZone { get; init; }
        public string? voterSection { get; init; }
        public string? militaryCertificateType { get; init; }
        public string? militaryCategory { get; init; }
        public string? militaryDispensation { get; init; }
        public string? nitNumber { get; init; }
        public string? ctpsState { get; init; }
        public bool? isInformalWorker { get; init; }
        public string? pisNumber { get; init; }
        public string? pisIssueDate { get; init; }
        public string? pisState { get; init; }
        public string? ruralProducerRegistration { get; init; }
        public string? ruralProducerState { get; init; }
        public string? cifIssuer { get; init; }
        public string? cifState { get; init; }
        public string? cifNumber { get; init; }
        public string? birthState { get; init; }
        public string? passportIssuer { get; init; }
        public string? passportState { get; init; }
        public string? passportExpirationDate { get; init; }
        public string? ricNumber { get; init; }
        public string? ricState { get; init; }
        public string? ricIssuer { get; init; }
        public int? ricCityCode { get; init; }
        public string? ricIssueDate { get; init; }
        public string? ricExpirationDate { get; init; }
        public string? foreignDocumentNumber { get; init; }
        public string? nickname { get; init; }
        public string? crcNumber { get; init; }
        public string? driverLicenseSecurityNumber { get; init; }
        public string? driverLicenseRegistrationNumber { get; init; }
        public string? crcRegistrationDate { get; init; }
        public string? crcExpirationDate { get; init; }
        public string? socialName { get; init; }
        public int? homeTown { get; init; }
    }
}
