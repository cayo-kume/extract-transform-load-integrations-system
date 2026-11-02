using Application.Shared.Abstractions.Handlers.Commands;

namespace Application.Shared.Handlers.Commands
{
    public class CommandHandler : ICommandHandler
    {
        public string CreateInsertRecordQuery()
        {
            return $@"INSERT INTO [auditing].[Logs] ([IdJob], [IdType], [Company], [StartDate], [EndDate], [Execution])
                      VALUES (@IdJob, @IdType, @Company, @StartDate, @EndDate, @Execution)";
        }
    }
}
