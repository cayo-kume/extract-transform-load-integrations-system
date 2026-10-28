namespace Domain.Totvs.Dtos.Writes.Products.Data
{
    /// <summary>
    /// produtos alcançados por PutBranchInfo e PutData (ProductDataFilter): nesta versão só a lista de produtos ou a de códigos de barras,
    /// até 500 (PR9); groupCode e referenceCode alcançariam um grupo inteiro e são recusados
    /// </summary>
    public class ProductDataFilter
    {
        public List<int>? productCodeList { get; init; }
        public List<string>? barCodeList { get; init; }
        public string? groupCode { get; init; }
        public string? referenceCode { get; init; }
    }
}
