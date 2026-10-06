namespace Application.Totvs.Abstractions.Services
{
    public interface IPersonService
    {
        public Task<bool?> PostSearchCustomers();
        public Task<bool?> PostSearchLegalEntity();
        public Task<bool?> GetBranchesList();
    }
}
