namespace Domain.Totvs.Dtos.Writes.Products
{
    /// <summary>
    /// tipo de inspeção por empresa (campo "Inspeção" do PRDFM236): vai como número (T3)
    /// </summary>
    public enum InspectionType
    {
        NoInpection = 0,   // sem inspeção (grafia do swagger)
        MandatoryInspection = 1,   // inspeção obrigatória
        NonMandatoryInspection = 2   // inspeção não obrigatória
    }
}
