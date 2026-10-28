namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// tipo do comissionado do item (CommissionedType): vai como número (T3)
    /// </summary>
    public enum CommissionedType
    {
        Representative = 1,   // representante
        Supervisor = 2,   // supervisor
        Broker = 3,   // corretor
        General = 90   // geral
    }
}
