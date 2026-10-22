namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// ativo da pessoa (a documentação escreve "Id"; a API real devolve "id", como no DTO de leitura)
    /// </summary>
    public class Asset
    {
        public string? id { get; init; }

        public string? name { get; init; }

        public string? label { get; init; }
    }
}
