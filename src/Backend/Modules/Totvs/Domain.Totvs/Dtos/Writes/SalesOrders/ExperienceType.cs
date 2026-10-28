namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// tipo de experiência do pedido (ExperienceType), com os números da descrição do swagger: vai como número (SO11)
    /// </summary>
    public enum ExperienceType
    {
        Ecommerce = 1,          // comércio eletrônico
        ShipFromStore = 2,      // a loja envia
        ClickAndCollect = 3,    // clique e colete
        PickupStore = 4,        // coleta na loja
        InfiniteShelf = 5       // prateleira infinita
    }
}
