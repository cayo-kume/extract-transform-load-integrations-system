namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsSellersService
    {
        /// <summary>
        /// sincroniza os vendedores de todas as lojas da holding pelo POST seller/v2/search; só grava os novos ou alterados
        /// </summary>
        public Task<bool?> PostSearchSellers();

        /// <summary>
        /// sincroniza as regiões de atuação, com os estados, CEPs e cidades, pelas quatro rotas GET seller/v2/operational-area*;
        /// lidas inteiras, só grava as regiões novas ou cujo hash mudou
        /// </summary>
        public Task<bool?> GetOperationalArea();
    }
}
