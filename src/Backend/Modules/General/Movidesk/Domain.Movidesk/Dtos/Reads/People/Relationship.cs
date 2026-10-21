namespace Domain.Movidesk.Dtos.Reads.People
{
    public class Relationship
    {
        public string id { get; init; }
        public string name { get; init; }
        public string slaAgreement { get; init; }
        public string forceChildrenToHaveSomeAgreement { get; init; }
        public string allowAllServices { get; init; }
        public string includeInParents { get; init; }
        public string loadChildOrganizations { get; init; }
        public List<Service>? services { get; init; }
    }
}
