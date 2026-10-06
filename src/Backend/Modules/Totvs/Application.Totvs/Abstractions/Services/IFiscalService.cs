namespace Application.Totvs.Abstractions.Services
{
    public interface IFiscalService
    {
        public Task<bool?> PostSearchInvoices();
    }
}
