namespace Domain.Totvs.Dtos.Reads.People.ReceptionSales
{
    /// <summary>
    /// recepção de venda do GET person/v2/reception-sale/search (ReceptionSaleDataModel): entrada do cliente na loja (PESFL023).
    /// Os telefones do cliente não entram: já estão em TotvsPeoplePhones
    /// </summary>
    public class ReceptionSale
    {
        public string? branchCode { get; init; }
        public string? movementDate { get; init; }
        public string? sequence { get; init; }
        public string? customerCode { get; init; }
        public string? customerName { get; init; }
        public string? sellerCode { get; init; }
        public string? sellerName { get; init; }
        public string? representativeCode { get; init; }
        public string? representativeName { get; init; }
        public string? guideCode { get; init; }
        public string? guideName { get; init; }
        public string? receptionType { get; init; }
        public string? checkInTime { get; init; }
        public string? observation { get; init; }

        // formas de pagamento do cliente (1 - Fatura, 2 - Cheque...)
        public List<string>? documentTypeList { get; init; }
    }
}
