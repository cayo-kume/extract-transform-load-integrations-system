namespace Application.Totvs.Abstractions.Services
{
    public interface IAccountsPayableService
    {
        public Task<bool?> PostSearchDuplicates();
        public Task<bool?> PostSearchComissionsPaid();
    }
}
