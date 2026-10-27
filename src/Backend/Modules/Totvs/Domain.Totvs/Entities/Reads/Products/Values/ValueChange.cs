namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// maior data de alteração gravada de um produto numa loja, para o cache de estado das leituras de valores
    /// </summary>
    public class ValueChange
    {
        public int productCode { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public ValueChange() { }

        public ValueChange(int productCode, DateTime? maxChangeFilterDate) =>
            (this.productCode, this.maxChangeFilterDate) = (productCode, maxChangeFilterDate);
    }
}
