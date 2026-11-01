namespace WebApp.Models
{
    /// <summary>
    /// Pedido de "Executar agora" ainda não atendido pelo host. Na fase 5, o host lê o pedido no próximo ciclo da agenda e
    /// enfileira a execução; o painel não chama o host (decisão: sem API).
    /// </summary>
    public class RunRequestModel
    {
        public DateTime RequestedAt { get; set; }
        public string? RequestedBy { get; set; }
    }
}
