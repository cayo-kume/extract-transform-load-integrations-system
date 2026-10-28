namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// pessoas da Totvs (API Person): filiais, pessoas físicas e jurídicas
    /// </summary>
    public interface ITotvsPeopleService
    {
        /// <summary>
        /// sincroniza as empresas (lojas) da holding pelo GET person/v2/branchesList, com endereços; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> GetBranchesList();

        /// <summary>
        /// sincroniza as pessoas físicas pelo POST person/v2/individuals/search, com as estatísticas de todas as lojas; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchCustomers();

        /// <summary>
        /// sincroniza as pessoas jurídicas pelo POST person/v2/legal-entities/search, com as estatísticas de todas as lojas; só grava as novas ou alteradas
        /// </summary>
        public Task<bool?> PostSearchLegalEntity();

        /// <summary>
        /// busca uma pessoa física por código (POST person/v2/individuals/search com personCodeList) e grava sempre, sem consultar
        /// o cache; a chamada é registrada em totvs.TotvsRequests. true gravada, false não existe na Totvs, null empresa não
        /// cadastrada ou resposta inválida
        /// </summary>
        public Task<bool?> GetCustomerByCode(string user_name, string doc_company, int code);

        /// <summary>
        /// busca uma pessoa jurídica por código (POST person/v2/legal-entities/search com personCodeList); mesmo contrato do GetCustomerByCode
        /// </summary>
        public Task<bool?> GetLegalEntityByCode(string user_name, string doc_company, int code);

        /// <summary>
        /// devolve os dados de uma loja pelo código ou CNPJ (GET person/v2/branches/{branchId}), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetBranches(string user_name, string doc_company, string branchId);

        /// <summary>
        /// devolve as estatísticas de compra e de crédito de um cliente nas lojas (GET person/v2/person-statistics), sem gravar
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> GetPersonStatistics(string user_name, string doc_company, int? customerCode, string? customerCpfCnpj, List<int>? branchCodes);

        /// <summary>
        /// inclui mensagens para uma pessoa (POST person/v2/messages); a resposta traz a sequence de cada uma. Não idempotente: o mesmo
        /// corpo em 10 minutos volta 409 (T5)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostMessages(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Messages.InsertMessagePerson body);

        /// <summary>
        /// altera uma mensagem da pessoa (PUT person/v2/messages/change): descrição, situação ou prioridade
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PutChangeMessages(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Messages.UpdateMessagePerson body);

        /// <summary>
        /// exclui uma mensagem da pessoa (DELETE person/v2/messages), em definitivo e só se não lida (a Totvs recusa as outras); cancelar pelo PutChangeMessages é o caminho normal (PE8)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> DeleteMessages(string user_name, string doc_company, int personCode, int sequence);

        /// <summary>
        /// cria ou altera um cliente pessoa física pelo cpf (POST person/v2/individual-customers); a resposta traz o customerCode. Os documentos,
        /// a conta e a chave PIX vão mascarados para totvs.TotvsRequests (T6)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostIndividualCustomers(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Customers.IndividualCustomer body);

        /// <summary>
        /// cria ou altera um cliente pessoa jurídica pelo cnpj (POST person/v2/legal-customers); a resposta traz o customerCode
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostLegalCustomers(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Customers.LegalCustomer body);

        /// <summary>
        /// tira classificações de uma pessoa (POST person/v2/classification-relationship/remove)
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> PostRemoveClassificationRelationship(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Classifications.RemoveClassificationRelationship body);
    }
}
