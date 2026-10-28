namespace Domain.Totvs.Dtos.Writes.People.Representatives
{
    /// <summary>
    /// corpo do POST person/v2/customer-representatives (InsertRelateCustomerRepresentativeCommand): relaciona clientes a um representante. O
    /// representante vai pelo código ou pelo CPF/CNPJ, e os clientes por uma das duas listas. A API não tem rota para desfazer o vínculo
    /// </summary>
    public class CustomerRepresentative
    {
        public int? representativeCode { get; init; }
        public string? representativeCpfCnpj { get; init; }
        public List<int>? customerCodeList { get; init; }
        public List<string>? customerCpfCnpjList { get; init; }
    }
}
