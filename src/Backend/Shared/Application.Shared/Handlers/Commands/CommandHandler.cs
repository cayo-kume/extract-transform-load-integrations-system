using Application.Shared.Abstractions.Handlers.Commands;

namespace Application.Shared.Handlers.Commands
{
    public class CommandHandler : ICommandHandler
    {
        public string CreateInsertRecordQuery()
        {
            return $@"INSERT INTO [auditing].[Logs] ([IdJob], [IdType], [StartDate], [EndDate], [Execution])
                      VALUES (@IdJob, @IdType, @StartDate, @EndDate, @Execution)";
        }
    }
}
