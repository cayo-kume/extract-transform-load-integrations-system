namespace Application.Totvs.Abstractions.Services
{
    public interface IProductService
    {
        public Task<bool?> PostSearchReferences();
    }
}
