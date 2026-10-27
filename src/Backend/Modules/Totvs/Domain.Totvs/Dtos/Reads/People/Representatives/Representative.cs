namespace Domain.Totvs.Dtos.Reads.People.Representatives
{
    /// <summary>
    /// representante do POST person/v2/representatives/search (RepresentativeDataModel). As filhas comuns com a PF usam os DTOs
    /// de People/Persons
    /// </summary>
    public class Representative
    {
        public string? code { get; init; }
        public string? cpfCnpj { get; init; }
        public string? isInactive { get; init; }
        public string? name { get; init; }
        public string? supervisorCode { get; init; }
        public string? supervisorName { get; init; }
        public string? branchInsertCode { get; init; }
        public string? maxChangeFilterDate { get; init; }

        public List<Persons.Address>? addresses { get; init; }
        public List<Persons.Phone>? phones { get; init; }
        public List<Persons.Email>? emails { get; init; }
        public List<Persons.Observation>? observations { get; init; }
        public List<Persons.AdditionalField>? additionalFields { get; init; }
        public List<Persons.Classification>? classifications { get; init; }
        public List<RepresentativeCustomer>? customers { get; init; }
        public List<RepresentativePriceTable>? priceTables { get; init; }
        public List<RepresentativeSalesRegion>? salesRegions { get; init; }
        public List<RepresentativeParameter>? representativeParameters { get; init; }
    }
}
