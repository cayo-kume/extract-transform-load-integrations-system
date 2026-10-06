

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxRotinaOrigem
    {
        public string? codigo_rotina { get; private set; }
        public string? portal { get; private set; }
        public string? descricao_rotina { get; private set; }
        public string? timestamp { get; private set; }

        public LinxRotinaOrigem() { }

        public LinxRotinaOrigem(
            string? codigo_rotina,
            string? portal,
            string? descricao_rotina,
            string? timestamp
        )
        {
            this.codigo_rotina = codigo_rotina;
            this.descricao_rotina = descricao_rotina;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}