namespace Domain.Totvs.Dtos.Writes.Products
{
    /// <summary>
    /// origem da mercadoria do CST (campo "CST" do PRDFM236): vai como número (T3); entra no cálculo de impostos da nota
    /// </summary>
    public enum TaxStatusCodeType
    {
        National = 0,   // nacional, exceto 3, 4, 5 e 8
        Foreing = 1,   // estrangeira, importação direta, exceto 6
        ForeingDomesticallyAcquired = 2,   // estrangeira, adquirida no mercado interno, exceto 7
        NationalWithImportContent = 3,   // nacional, conteúdo de importação acima de 40% e até 70%
        NationalProduction = 4,   // nacional, produção pelos processos básicos
        NationalWithLowerImportContent = 5,   // nacional, conteúdo de importação até 40%
        ForeingDirectImport = 6,   // estrangeira, importação direta, sem similar (CAMEX)
        ForeingAcquiredInternally = 7,   // estrangeira, mercado interno, sem similar (CAMEX)
        NationalWithSuperiorImportContent = 8   // nacional, conteúdo de importação acima de 70%
    }
}
