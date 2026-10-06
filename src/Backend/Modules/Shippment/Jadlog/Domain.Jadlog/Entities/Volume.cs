namespace Domain.Jadlog.Entities
{
    public class Volume
    {
        public DateTime lastupdateon { get; set; }
        public string shipmentId { get; set; }
        public decimal peso { get; set; }
        public decimal altura { get; set; }
        public decimal largura { get; set; }
        public decimal comprimento { get; set; }

        public Volume() { }

        public Volume(DTOs.Volume volume, string shipmentId)
        {
            this.lastupdateon = DateTime.Now;
            this.peso = volume.peso;
            this.altura = volume.altura;
            this.largura = volume.largura;
            this.comprimento = volume.comprimento;
            this.shipmentId = shipmentId;
        }
    }
}
