namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsSellersService
    {
        /// <summary>
        /// sincroniza os vendedores de todas as lojas da holding pelo POST seller/v2/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchSellers();
    }
}
