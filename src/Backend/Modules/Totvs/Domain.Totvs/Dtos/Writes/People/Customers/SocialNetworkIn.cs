namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// rede social (SocialNetworkInDto): sequence só na alteração; sem ela, inclui
    /// </summary>
    public class SocialNetworkIn
    {
        public int? sequence { get; init; }
        public int? typeCode { get; init; }
        public string? address { get; init; }
    }
}
