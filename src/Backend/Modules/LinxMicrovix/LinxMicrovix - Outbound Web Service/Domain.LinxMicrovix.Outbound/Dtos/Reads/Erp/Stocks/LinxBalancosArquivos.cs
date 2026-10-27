namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxBalancosArquivos: os dados dos arquivos de balanços. (colunas da v274: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class LinxBalancosArquivos
    {
        public string? portal { get; init; }
        public string? id_balancos_arquivos { get; init; }
        public string? codigoproduto { get; init; }
        public string? qtdeproduto { get; init; }
        public string? lote { get; init; }
        public string? timestamp { get; init; }
    }
}
