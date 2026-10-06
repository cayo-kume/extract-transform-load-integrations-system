using Azure.Storage.Blobs;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys.LinxMicrovix
{
    public class LinxReshopSaldoRepository : ILinxReshopSaldoRepository
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _blobFolder;

        public LinxReshopSaldoRepository(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString not configured.");
            _containerName = configuration["AzureStorage:ContainerName"]
                ?? throw new InvalidOperationException("AzureStorage:ContainerName not configured.");
            _blobFolder = configuration["LinxReshopExcel:BlobFolder"]
                ?? "Digital/Reshop";
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobs = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: _blobFolder))
            {
                if (blobItem.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    blobs.Add(blobItem.Name);
            }

            return blobs;
        }

        public async Task<Stream> GetExcelStreamAsync(string blobPath)
        {
            var blob = new BlobClient(_connectionString, _containerName, blobPath);

            var memoryStream = new MemoryStream();
            await blob.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task DeleteBlobAsync(string blobPath)
        {
            var blob = new BlobClient(_connectionString, _containerName, blobPath);
            await blob.DeleteIfExistsAsync();
        }
    }
}
