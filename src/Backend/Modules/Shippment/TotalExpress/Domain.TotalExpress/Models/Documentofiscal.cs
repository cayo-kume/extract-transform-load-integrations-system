namespace Domain.TotalExpress.Models
{
    public class Documentofiscal
    {
        public string pedido { get; private set; }
        public string serie { get; private set; }
        public string numero { get; private set; }

        public Documentofiscal() { }

        public Documentofiscal(Dtos.Return.Documentofiscal documento, string pedido)
        {
            this.pedido = pedido;
            this.serie = documento.serie;
            this.numero = documento.numero;
        }
    }
}
