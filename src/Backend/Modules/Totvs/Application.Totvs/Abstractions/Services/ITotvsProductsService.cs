namespace Application.Totvs.Abstractions.Services
{
    public interface ITotvsProductsService
    {
        /// <summary>
        /// sincroniza as referências e produtos pelo POST product/v2/references/search, uma chamada por marca (matrizes 1 e 101
        /// como empresa base, filtradas pela classificação 6000 da marca); só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchReferences();
    }
}
