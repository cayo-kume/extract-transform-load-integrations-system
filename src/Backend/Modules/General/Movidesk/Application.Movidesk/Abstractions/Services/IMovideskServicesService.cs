namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskServicesService
    {
        public Task<bool?> GetService();
        public Task<bool?> GetServices();
    }
}
