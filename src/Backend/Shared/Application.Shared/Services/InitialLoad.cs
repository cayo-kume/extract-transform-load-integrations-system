using Application.Shared.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace Application.Shared.Services
{
    public class InitialLoad : IInitialLoad
    {
        // sem o pacote de binder: a chave ausente, zero, negativa ou inválida conta como desligada
        public InitialLoad(IConfiguration configuration) =>
            MaxMonths = int.TryParse(configuration["InitialLoad:MaxMonths"], out var months) && months > 0 ? months : null;

        public int? MaxMonths { get; }

        public DateTime Since(DateTime since, DateTime now)
        {
            if (MaxMonths is not int months)
                return since;

            var limit = now.AddMonths(-months);

            return since < limit ? limit : since;
        }
    }
}
