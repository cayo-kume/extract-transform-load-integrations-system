namespace Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix
{
    public interface ILinxReshopSaldoRepository
    {
        Task<List<string>> ListBlobsAsync();
        Task<Stream> GetExcelStreamAsync(string blobPath);
        Task DeleteBlobAsync(string blobPath);
    }
}
