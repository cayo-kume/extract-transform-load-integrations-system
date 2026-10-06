namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxCfopFiscal
    {
        public string? portal { get; set; }
        public string? id_cfop_fiscal { get; set; }
        public string? cfop_fiscal { get; set; }
        public string? descricao { get; set; }
        public string? excluido { get; set; }
        public string? timestamp { get; set; }

        public LinxCfopFiscal()
        {
        }

        public LinxCfopFiscal(string? portal, string? id_cfop_fiscal, string? cfop_fiscal, string? descricao, string? excluido, string? timestamp)
        {
            this.portal = portal;
            this.id_cfop_fiscal = id_cfop_fiscal;
            this.cfop_fiscal = cfop_fiscal;
            this.descricao = descricao;
            this.excluido = excluido;
            this.timestamp = timestamp;
        }
    }
}
