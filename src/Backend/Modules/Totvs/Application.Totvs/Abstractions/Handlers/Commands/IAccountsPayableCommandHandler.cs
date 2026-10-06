using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface IAccountsPayableCommandHandler
    {
        string CreateGetExistingDuplicatesQuery(List<string?> registros);
        string CreateGetExistingComissionsPaidQuery(List<string?> registros);
    }
}
