namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// corpo do POST e do PATCH de pessoas: só os campos que podem ser escritos. Tudo anulável e serializado sem nulos:
    /// no PATCH, null = campo não enviado (não muda); lista vazia = lista apagada; lista com itens = lista substituída.
    /// Sem password: a integração não recebe nem repassa senhas (uma senha enviada no JSON é ignorada)
    /// </summary>
    public class Person
    {
        // Cód. Ref.: só na criação (somente leitura depois)
        public string? id { get; init; }

        public string? codeReferenceAdditional { get; init; }

        public bool? isActive { get; init; }

        // 1 = Pessoa, 2 = Empresa, 4 = Departamento
        public int? personType { get; init; }

        // 1 = Agente, 2 = Cliente, 3 = Agente e Cliente
        public int? profileType { get; init; }

        // perfil de acesso cadastrado; obrigatório para agente
        public string? accessProfile { get; init; }

        // razão social; obrigatória para empresa sem businessName
        public string? corporateName { get; init; }

        // nome (pessoa e departamento) ou nome fantasia (empresa)
        public string? businessName { get; init; }

        public string? businessBranch { get; init; }

        // 11 dígitos para pessoa, 14 para empresa, nenhum para departamento
        public string? cpfCnpj { get; init; }

        public string? userName { get; init; }

        public string? role { get; init; }

        // Cód. Ref. do superior
        public string? bossId { get; init; }

        public string? classification { get; init; }

        // ex.: pt-BR
        public string? cultureId { get; init; }

        // padrão IANA, ex.: America/Sao_Paulo
        public string? timeZoneId { get; init; }

        public string? authenticateOn { get; init; }

        public string? observations { get; init; }

        public List<Address>? addresses { get; init; }

        public List<Contact>? contacts { get; init; }

        public List<Email>? emails { get; init; }

        // nomes das equipes; ao menos uma para agente
        public List<string>? teams { get; init; }

        public List<Relationship>? relationships { get; init; }

        public List<CustomFieldValue>? customFieldValues { get; init; }

        public List<Asset>? atAssets { get; init; }
    }
}
