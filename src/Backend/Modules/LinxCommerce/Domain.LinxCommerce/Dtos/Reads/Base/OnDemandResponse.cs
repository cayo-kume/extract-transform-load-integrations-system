namespace Domain.LinxCommerce.Dtos.Reads.Base
{
    /// <summary>
    /// resultado de uma chamada sob demanda: o status do Linx Commerce (null quando a chamada lançou exceção), o DTO e o JSON
    /// sem os campos sensíveis (os dois nulos quando a resposta não é 2xx ou não é um JSON válido)
    /// </summary>
    public class OnDemandResponse<T> where T : class
    {
        public int? StatusCode { get; init; }
        public T? Dto { get; init; }
        public string? Json { get; init; }

        public bool IsValid => StatusCode is >= 200 and < 300 && Dto is not null;
    }
}
