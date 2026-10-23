namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (person); vira colunas da nota com o prefixo person
    /// </summary>
    public class Person
    {
        public string? personCode { get; init; }
        public string? personName { get; init; }
        public string? personType { get; init; }
        public string? postOfficeBox { get; init; }
        public string? addressNumber { get; init; }
        public string? cep { get; init; }
        public string? address { get; init; }
        public string? publicPlace { get; init; }
        public string? complement { get; init; }
        public string? neighborhood { get; init; }
        public string? city { get; init; }
        public string? stateAbbreviation { get; init; }
        public string? foneNumber { get; init; }
        public string? rgIe { get; init; }
        public string? personCpfCnpj { get; init; }
        public string? cityCode { get; init; }
    }
}
