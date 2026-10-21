namespace Domain.Shared.Abstractions.Api
{
    public interface IApiClient
    {
        Task<string?> GetAsync(string route, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null);
        Task<string?> PostAsync(string route, object? body = null, Dictionary<string, string>? headers = null, string? contentType = null);
        Task<string?> PutAsync(string route, object? body = null, Dictionary<string, string>? headers = null);
        Task<string?> DeleteAsync(string route, object? body = null, Dictionary<string, string>? headers = null);
        Task<string?> PostFormAsync(string route, Dictionary<string, string> formData, Dictionary<string, string>? headers = null);
        Task<string?> PostMultipartAsync(string route, Dictionary<string, string> fields, string fileField, byte[] fileContent, string fileName, string fileContentType, Dictionary<string, string>? headers = null);
    }
}
