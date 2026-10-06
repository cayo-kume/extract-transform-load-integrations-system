namespace Domain.Jadlog.Entities
{
    public class Recebedor
    {
        public string nome { get; set; }
        public string doc { get; set; }
        public DateTime? data { get; set; }

        public Recebedor() { }

        public Recebedor(DTOs.Recebedor recebedor)
        {
            if (recebedor != null)
            {
                this.nome = recebedor.nome;
                this.doc = recebedor.doc;
                this.data = recebedor.data;
            }
            else
            {
                this.nome = null;
                this.doc = null;
                this.data = null;
            }
        }
    }
}
