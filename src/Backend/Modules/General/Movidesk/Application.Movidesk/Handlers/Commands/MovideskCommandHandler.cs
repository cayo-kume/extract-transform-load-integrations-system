using Domain.Movidesk.Interfaces.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskCommandHandler : IMovideskCommandHandler
    {
        public string CreateGetTokenQuery()
        {
            return @$"SELECT DISTINCT
                     TOKEN
                     FROM GENERAL.PARAMETROS_MOVIDESK";
        }
    }
}
