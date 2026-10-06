namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxSetores
    {
        public string? id_setor { get; set; }
        public string? desc_setor { get; set; }
        public string? timestamp { get; set; }
        public string? ativo { get; set; }
        public string? codigo_integracao_ws { get; set; }

        public LinxSetores() { }

        public LinxSetores(
            string? id_setor,
            string? desc_setor,
            string? timestamp,
            string? ativo,
            string? codigo_integracao_ws
        )
        {
            this.id_setor = id_setor;
            this.desc_setor = desc_setor;
            this.timestamp = timestamp;
            this.ativo = ativo;
            this.codigo_integracao_ws = codigo_integracao_ws;
        }
    }
}