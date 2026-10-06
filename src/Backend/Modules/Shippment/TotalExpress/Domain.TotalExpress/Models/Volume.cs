namespace Domain.TotalExpress.Models
{
    public class Volume
    {
        public string pedido { get; set; }
        public string awb { get; set; }
        public string rota { get; set; }
        public string codigoBarras { get; set; }

        public Volume() { }

        public Volume(Dtos.Return.Volume volume, string pedido)
        {
            this.pedido = pedido;
            this.awb = volume.awb;
            this.rota = volume.rota;
            this.codigoBarras = volume.codigoBarras;
        }
    }
}
