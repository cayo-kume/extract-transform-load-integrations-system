namespace Domain.Movidesk.Dtos.Writes.Services
{
    /// <summary>
    /// corpo do POST e do PATCH de serviços: só os campos que podem ser escritos. Tudo anulável e serializado sem nulos:
    /// no PATCH, null = campo não enviado (não muda); categories vazia = categorias apagadas; com itens = substituídas.
    /// Sem id: a Movidesk gera o id na criação, e no PATCH o id vai na rota (um id enviado no JSON é ignorado)
    /// </summary>
    public class Service
    {
        public string? name { get; init; }

        public string? description { get; init; }

        public int? parentServiceId { get; init; }

        // 0 = Público, 1 = Interno, 2 = Públicos e internos
        public int? serviceForTicketType { get; init; }

        // 1 = Agente, 2 = Cliente, 3 = Agente e Cliente (o 0 da leitura não é documentado e não é aceito)
        public int? isVisible { get; init; }

        // 1 = Agente, 2 = Cliente, 3 = Agente e Cliente
        public int? allowSelection { get; init; }

        // falso: o ticket não pode ser concluído com este serviço selecionado
        public bool? allowFinishTicket { get; init; }

        public bool? isActive { get; init; }

        // nome da macro executada ao selecionar o serviço
        public string? automationMacro { get; init; }

        public string? defaultCategory { get; init; }

        public string? defaultUrgency { get; init; }

        public bool? allowAllCategories { get; init; }

        // nomes das categorias; obrigatória quando allowAllCategories = false
        public List<string>? categories { get; init; }
    }
}
