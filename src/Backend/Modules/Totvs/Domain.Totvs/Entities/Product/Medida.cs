namespace Domain.Totvs.Entities.Product
{
    public class Medida
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public string sku { get; private set; }
        public string chave { get; private set; }
        public string valor { get; private set; }

        public Medida() { }

        public Medida(int referenceId, string colorCode, string sku, Dtos.Product.Medida medidaDto)
        {
             this.referenceId = referenceId;
             this.colorCode = colorCode;
             this.sku = sku;
             chave = medidaDto.chave?.Length > 60 ? medidaDto.chave.Substring(0, 60) : medidaDto.chave;
             valor = medidaDto.valor?.Length > 60 ? medidaDto.valor.Substring(0, 60) : medidaDto.valor;
        }
    }
}
