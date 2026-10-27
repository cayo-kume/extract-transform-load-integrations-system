namespace Application.Totvs.Services
{
    /// <summary>
    /// marcas dos produtos: a empresa base de cada uma (option.branchInfoCode das referências aceita uma empresa só) e o código da marca
    /// na classificação 6000 (SETOR). Sem esse filtro as chamadas trazem o catálogo inteiro (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4).
    /// Códigos da classificação conferidos na API: 2 = MISHA, 4 = OPEN ERA (1 = GERAL, itens de consumo, fica de fora). Uma marca nova
    /// entra aqui. Usado pelas referências e pelas leituras de preço, custo e saldo
    /// </summary>
    public static class TotvsProductBrands
    {
        public const int ClassificationType = 6000;

        public static readonly IReadOnlyDictionary<int, string> BaseBranchCodes = new Dictionary<int, string>
        {
            { 1, "2" },     // Misha
            { 101, "4" }    // Open Era
        };
    }
}
