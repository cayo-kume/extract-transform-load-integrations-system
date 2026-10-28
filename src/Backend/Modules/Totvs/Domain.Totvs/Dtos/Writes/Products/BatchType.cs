namespace Domain.Totvs.Dtos.Writes.Products
{
    /// <summary>
    /// tipo de lote (campo "Tipo lote" do PRDFM059): vai como número (T3)
    /// </summary>
    public enum BatchType
    {
        Fabric = 1,   // tecido
        Yarn = 2,   // fio
        Mesh = 3,   // malha
        General = 4,   // geral
        Single = 5   // único
    }
}
