using Application.Shared.Abstractions.Services;

namespace Application.Shared.Services
{
    /// <summary>
    /// o padrão: nada a conferir (os hosts de agenda pedem a trava antes de executar)
    /// </summary>
    public class NoJobExecutionGuard : IJobExecutionGuard
    {
        public void BeforeLogStart(Enum job, string company)
        {
        }
    }
}
