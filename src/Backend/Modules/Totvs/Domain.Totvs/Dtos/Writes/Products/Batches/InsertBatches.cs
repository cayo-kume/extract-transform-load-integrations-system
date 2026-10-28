namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// corpo do POST product/v2/batch/create (InsertBatchProductCommand): inclui lotes e itens de lote. Sem batchCode, a Totvs gera:
    /// repetir cria outro lote, daí a trava (T5). Mexe em saldo de matéria-prima e inspeção
    /// </summary>
    public class InsertBatches
    {
        // grafia da API
        public List<BatchProduct>? batchs { get; init; }
    }
}
