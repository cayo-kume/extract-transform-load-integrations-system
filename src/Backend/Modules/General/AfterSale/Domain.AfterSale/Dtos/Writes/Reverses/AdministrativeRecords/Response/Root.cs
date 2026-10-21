using Domain.AfterSale.Dtos.Writes.Base;

namespace Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Response
{
    /// <summary>
    /// response of POST v3/api/reverses/{id}/administrative-records
    /// </summary>
    public class Root : ResponseRoot
    {
        public Data? data { get; init; }
    }
}
