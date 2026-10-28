namespace Domain.Totvs.Dtos.Writes.Products
{
    /// <summary>
    /// tipo de item do SPED por empresa (campo "Tp. item SPED" do PRDFM236): vai como número (T3)
    /// </summary>
    public enum SpedItemType
    {
        MerchandiseForResale = 0,   // mercadoria para revenda
        RawMaterial = 1,   // matéria-prima
        Packaging = 2,   // embalagem
        ProductInProcess = 3,   // produto em processo
        FinishedProduct = 4,   // produto acabado
        ByProduct = 5,   // subproduto
        IntermediateProduct = 6,   // produto intermediário
        MaterialForUseAndConsumption = 7,   // material de uso e consumo
        FixedAsset = 8,   // ativo imobilizado
        Services = 9,   // serviços
        OtherInputs = 10,   // outros insumos
        Others = 99   // outras
    }
}
