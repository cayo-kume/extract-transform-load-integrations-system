namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// estado gravado de uma loja numa das tabelas de valores (TotvsPrices, TotvsCosts, TotvsBalances): a maior data de alteração
    /// (null enquanto só houver a carga inicial, que é lida sem filter.change), o maior produto (onde a carga inicial continua) e o
    /// primeiro horário de gravação (o começo da carga inicial, de onde sai a primeira janela)
    /// </summary>
    public class ValueState
    {
        public DateTime? lastChange { get; private set; }
        public int? maxProductCode { get; private set; }
        public DateTime? firstIntegrated { get; private set; }

        public ValueState() { }
    }
}
