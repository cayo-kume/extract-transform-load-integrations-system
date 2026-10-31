using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Mensagens e registros falsos de uma execução, com os textos que os services gravam hoje em auditing.Messages,
    /// já no nível da proposta src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md.
    /// </summary>
    internal static class MockTexts
    {
        public static IReadOnlyList<MessageModel> Messages(MockExecution execution, string appTitle)
        {
            var job = execution.Job;
            var catalog = job.Catalog;
            var messages = new List<MessageModel>();

            MessageModel New(MessageLevel level, string message) => new()
            {
                IdExecutionMessage = (long)(MockTimeline.Hash(job, execution.K, 20 + messages.Count) % 900_000_000_000) + 1,
                IdJob = catalog.IdJob,
                IdType = catalog.IdType,
                Execution = execution.Execution,
                Level = level,
                Message = message
            };

            for (var i = 0; i < execution.Warnings; i++)
                messages.Add(New(MessageLevel.Warning, Warning(execution, appTitle, i)));

            if (execution.Errors > 0)
            {
                messages.Add(Error(execution, appTitle, New));
            }
            else if (execution.EndDate is not null)
            {
                messages.Add(New(MessageLevel.Information, catalog.IdType == 1
                    ? "Concluída com sucesso!"
                    : $"Concluída com sucesso: {execution.Records} registro(s) novo(s) ou atualizado(s) inserido(s)!"));
            }

            return messages;
        }

        public static RecordModel Record(MockExecution execution, int index)
        {
            var job = execution.Job;
            var company = Company(job, execution.K + index / 50);
            var key = 100000 + (long)(MockTimeline.Hash(job, execution.K, 1000 + index) % 900000);

            return new RecordModel
            {
                IdRecord = (long)(MockTimeline.Hash(job, execution.K, 5000 + index) % 900_000_000_000) + 1,
                IdJob = job.Catalog.IdJob,
                IdType = job.Catalog.IdType,
                Execution = execution.Execution,
                FieldKeyValue = $"{company} | {key}",
                RecordText = $"<R><doc_company>{company}</doc_company><id>{key}</id><timestamp>{execution.StartDate:yyyyMMddHHmmss}</timestamp></R>"
            };
        }

        private static string Warning(MockExecution execution, string appTitle, int index)
        {
            var job = execution.Job;
            var company = Company(job, execution.K + index);
            var pick = MockTimeline.Hash(job, execution.K, 30 + index) % 2;

            return appTitle switch
            {
                "LinxMicrovix" => pick == 0
                    ? $"Empresa {company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes."
                    : $"Empresa {company} não cadastrada em linxmicrovix.LinxMicrovixParameters ou sem a chave do ERP.",
                "Totvs" => pick == 0
                    ? $"Empresa {company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes."
                    : $"Sugestões de faturamento da loja {1 + MockTimeline.Hash(job, execution.K, 40) % 30} descartadas: a loja não está em totvs.TotvsBranches da empresa {company}.",
                "AfterSale" => $"CNPJ {company} não encontrado em AfterSaleParameters: {1 + MockTimeline.Hash(job, execution.K, 41) % 12} reversa(s) ignorada(s).",
                "Movidesk" => $"Ticket {50000 + MockTimeline.Hash(job, execution.K, 42) % 9000} (empresa {company}) não veio em todos os grupos do $expand: não gravado nesta execução.",
                "LinxCommerce" => $"Empresa {company} não cadastrada em linxcommerce.LinxCommerceParameters.",
                "DataWarehouse" => "Concluída com checagem bloqueante: veja dw_control.CheckResult.",
                _ => $"Empresa {company} sem cadastro de parâmetros."
            };
        }

        private static MessageModel Error(MockExecution execution, string appTitle, Func<MessageLevel, string, MessageModel> create)
        {
            var job = execution.Job;
            var catalog = job.Catalog;
            var company = Company(job, execution.K);

            if (catalog.IdType == 1)
            {
                var message = create(MessageLevel.Error, $"Concluída com Erro. Transaction (Process ID {50 + MockTimeline.Hash(job, execution.K, 50) % 200}) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.");
                message.SqlCommand = $"EXEC [dbo].[{catalog.Title}]";
                return message;
            }

            var route = catalog.Title?.Replace(appTitle, "") ?? "";

            if (MockTimeline.Hash(job, execution.K, 51) % 2 == 0)
            {
                return create(MessageLevel.Error, appTitle switch
                {
                    "LinxMicrovix" => $"Resposta de erro da API Linx Microvix em {route} (empresa {company}, loja {Company(job, execution.K + 7)}, timestamp {execution.StartDate:yyyyMMddHHmm}): Chave de acesso inválida para o CNPJ informado.",
                    "Totvs" => $"Erro da API Totvs em {route} (empresa {company}, página {1 + MockTimeline.Hash(job, execution.K, 52) % 40}): 503 Service Unavailable",
                    _ => $"Resposta inválida da API {appTitle} em {route} (empresa {company}): <html><body>Bad Gateway</body></html>"
                });
            }

            var table = $"untreated.{catalog.Title?.Replace("Get", "")}";
            var error = create(MessageLevel.Error, $"Erro ao realizar Bulk Insert em {table}");
            error.ExceptionMessage = $"Violation of PRIMARY KEY constraint 'PK_{catalog.Title}'. Cannot insert duplicate key in object '{table}'. The duplicate key value is ({company}, {100000 + MockTimeline.Hash(job, execution.K, 53) % 900000}).";
            error.ExceptionStackTrace = string.Join(Environment.NewLine,
                "   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)",
                "   at Microsoft.Data.SqlClient.SqlBulkCopy.RunParser(BulkCopySimpleResultSet bulkCopyHandler)",
                "   at Microsoft.Data.SqlClient.SqlBulkCopy.WriteToServerInternalRestContinuedAsync(BulkCopySimpleResultSet internalResults, CancellationToken cts, TaskCompletionSource`1 source)",
                "   at Infrastructure.Shared.Repositorys.SharedRepository.BulkInsertIntoTableRaw(DataTable dataTable, String tableName, String schemaName)",
                $"   at Application.{appTitle}.Services.{appTitle}Service.{route}()",
                "   at Application.Shared.Handlers.Commands.CommandHandler.ExecuteAsync[T](Func`1 command)");
            error.SqlCommand = $"MERGE [{table.Replace("untreated", "trusted")}] AS TARGET{Environment.NewLine}USING (SELECT * FROM [{table}]) AS SOURCE{Environment.NewLine}ON (TARGET.doc_company = SOURCE.doc_company AND TARGET.id = SOURCE.id){Environment.NewLine}WHEN MATCHED THEN UPDATE SET TARGET.timestamp = SOURCE.timestamp{Environment.NewLine}WHEN NOT MATCHED BY TARGET THEN INSERT (doc_company, id, timestamp) VALUES (SOURCE.doc_company, SOURCE.id, SOURCE.timestamp);";
            return error;
        }

        // número de 14 dígitos com cara de CNPJ, gerado do hash (não é um CNPJ real)
        private static string Company(MockJob job, long seed) =>
            (10_000_000_000_000 + (long)(MockTimeline.Hash(job, seed % 6, 60) % 89_999_999_999_999)).ToString();
    }
}
