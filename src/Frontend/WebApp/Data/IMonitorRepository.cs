using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Leitura das tabelas auditing.* para o painel. Até a fase 5 do plano (docs/PLANO-WEBAPP.md), a única implementação é o
    /// MockMonitorRepository; depois, o MonitorRepository lê o banco principal com Dapper.
    /// </summary>
    public interface IMonitorRepository
    {
        /// <summary>
        /// O mesmo repositório, lendo e gravando os jobs e os logs de outra empresa (da holding de quem está logado; quem chama
        /// confere com LoggedCompany.CanSee). Sem ele, a empresa é a do seletor do topo. A visão "Todas as empresas" da holding
        /// lê cada empresa por aqui (CompanyView) e grava cada ação na empresa da linha.
        /// </summary>
        IMonitorRepository For(string company);

        /// <summary>
        /// Uma linha por aplicação: jobs, execuções, registros, avisos e erros no período.
        /// </summary>
        Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execuções por faixa de tempo de cada aplicação: o período dividido em buckets faixas iguais, alinhadas à hora
        /// (ou ao dia) cheia. previousWindow devolve a janela de mesmo tamanho imediatamente anterior, para comparação.
        /// </summary>
        Task<IReadOnlyList<AppBucketsModel>> GetExecutionBucketsAsync(Period period, int buckets, bool previousWindow = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Jobs com a última execução e a saúde, filtrados pela aplicação (já resolvida, ver JobModel.IdApp) e pelo tipo.
        /// </summary>
        Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execuções de um job no período, da mais recente para a mais antiga. page começa em 0.
        /// </summary>
        Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uma execução pelo Guid (a PK lógica de auditing.Logs), em qualquer período; nula se não existe.
        /// </summary>
        Task<ExecutionModel?> GetExecutionAsync(Guid execution, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default);

        Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Última execução de cada integração ativa, por gravidade: Erro, Atrasado, Rodando, falhas reconhecidas e Ok; em
        /// cada grupo, do início mais recente para o mais antigo, até top linhas. Entra quem rodou no período e quem está
        /// com Erro ou Atrasado, mesmo sem execução no período.
        /// </summary>
        Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mensagens a partir de um nível no período (Logs de Erros), com a aplicação, o job e o início da execução.
        /// </summary>
        Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(MessageQuery query, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Jobs de uma aplicação (query.IdApp) com mensagens no período, com os totais, a última ocorrência e a situação
        /// atual. Com query.Search, só os jobs com mensagens que têm o texto, e os totais contam só essas.
        /// </summary>
        Task<IReadOnlyList<MessageJobSummaryModel>> GetMessageJobsAsync(MessageQuery query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Causas das mensagens de um job (query.IdJob e query.IdType), da ocorrência mais recente para a mais antiga.
        /// </summary>
        Task<IReadOnlyList<MessageGroupModel>> GetMessageGroupsAsync(MessageQuery query, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Credenciais e parâmetros globais do módulo, para o diálogo de configurações da Visão geral. Somente leitura
        /// na fase 1; credencial vem mascarada.
        /// </summary>
        Task<IReadOnlyList<ModuleSettingModel>> GetModuleSettingsAsync(int idApp, CancellationToken cancellationToken = default);

        /// <summary>
        /// Liga ou desliga um job. Na implementação real, é um UPDATE em auditing.Jobs.IsActive na linha da empresa.
        /// </summary>
        Task SetJobActiveAsync(int idJob, int idType, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Agenda de uma integração: intervalo em minutos (0 = sob demanda), hora diária (com ela, o intervalo é 1440) e
        /// fila do host. Na implementação real, é um UPDATE em auditing.Jobs na linha da empresa; o host relê no próximo ciclo.
        /// </summary>
        Task SetJobScheduleAsync(int idJob, int idType, int intervalTimeInMinutes, TimeOnly? runningDailyAt, string queue, CancellationToken cancellationToken = default);

        /// <summary>
        /// Filas que os jobs já usam, para sugerir no campo Fila.
        /// </summary>
        Task<IReadOnlyList<string>> GetQueuesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Tipos de host de agenda no ar neste banco (auditing.SchedulingHosts), do que está no ar há mais tempo (o que
        /// agenda) para o mais recente.
        /// </summary>
        Task<IReadOnlyList<SchedulingHostModel>> GetSchedulingHostsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Liga ou desliga todos os jobs de um módulo (o chip "Ativa" do card).
        /// </summary>
        Task SetModuleActiveAsync(int idApp, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reconhece a falha atual do job (a última execução, ou a falta dela) com uma nota opcional. Vale até a próxima
        /// execução e por no máximo 24 h.
        /// </summary>
        Task AcknowledgeAsync(int idJob, int idType, string? note, CancellationToken cancellationToken = default);

        Task RemoveAcknowledgementAsync(int idJob, int idType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Pede ao host uma execução do job fora da agenda ("Executar agora"). Um pedido pendente por job.
        /// </summary>
        Task RequestRunAsync(int idJob, int idType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Imagem própria da aplicação para a empresa logada. A imagem padrão do módulo é a que não tem registro aqui.
        /// </summary>
        Task SetAppImageAsync(int idApp, string contentType, byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove a imagem própria; o card volta para a imagem padrão do módulo.
        /// </summary>
        Task RemoveAppImageAsync(int idApp, CancellationToken cancellationToken = default);
    }
}
