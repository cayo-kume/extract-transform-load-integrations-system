namespace Domain.Jadlog.Entities
{
    public class Erro
    {
        public int id { get; set; }
        public string pedido { get; set; }
        public string descricao { get; set; }
        public string detalhe { get; set; }

        public Erro() { }

        public Erro(DTOs.Erro erro, string pedido)
        {
            this.id = erro.id;
            this.pedido = pedido;
            this.descricao = erro.descricao;
        }
    }
}
