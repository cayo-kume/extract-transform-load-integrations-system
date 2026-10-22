namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// relacionamento da pessoa com uma organização (empresa ou departamento)
    /// </summary>
    public class Relationship
    {
        // Cód. Ref. da organização; hierarquia no padrão IdPai/IdFilho
        public string? id { get; init; }

        public string? slaAgreement { get; init; }

        public bool? forceChildrenToHaveSomeAgreement { get; init; }

        // quando não vem, a Movidesk assume true
        public bool? allowAllServices { get; init; }

        public bool? includeInParents { get; init; }

        public bool? loadChildOrganizations { get; init; }

        // ao menos um quando allowAllServices = false
        public List<RelationshipService>? services { get; init; }
    }
}
