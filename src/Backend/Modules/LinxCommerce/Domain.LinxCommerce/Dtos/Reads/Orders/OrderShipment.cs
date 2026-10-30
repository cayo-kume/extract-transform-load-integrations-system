namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderShipment do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderShipment
    {
        public string? OrderShipmentID { get; init; }
        public string? DeliveryMethodID { get; init; }
        public string? ShipmentNumber { get; init; }
        public string? ShipmentStatus { get; init; }
        public string? AssignUserId { get; init; }
        public string? AssignUserName { get; init; }
        public string? DockID { get; init; }

        public List<OrderPackage>? Packages { get; init; }
    }
}
