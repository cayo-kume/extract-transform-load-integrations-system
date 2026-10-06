namespace Application.LinxCommerce.Abstractions.Services
{
    public interface IPaymentTermService
    {
        public Task<string?> SearchPaymentTerm();
    }
}
