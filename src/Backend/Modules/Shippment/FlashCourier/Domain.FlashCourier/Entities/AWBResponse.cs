namespace Domain.FlashCourier.Entities
{
    public class AWBResponse
    {
        public string? statusRetorno { get; set; }
        public List<Hawb> hawbs { get; set; }

        public class Baixa
        {
            public string? grauParentesco { get; set; }
            public string? recebedor { get; set; }
            public string? rg { get; set; }
            public string? tentativas { get; set; }
            public string? dtBaixa { get; set; }
        }

        public class Hawb
        {
            public string? codigoCartao { get; set; }
            public string? hawbId { get; set; }
            public string? meuNumero { get; set; }
            public string? dtCol { get; set; }
            public string? dtPost { get; set; }
            public string? dtSla { get; set; }
            public string? contrato { get; set; }
            public List<Baixa> baixa { get; set; }
            public List<Historico> historico { get; set; }
        }

        public class Historico
        {
            public string? ocorrencia { get; set; }
            public string? eventoId { get; set; }
            public string? evento { get; set; }
            public string? arCorreios { get; set; }
            public string? frq { get; set; }
            public string? local { get; set; }
            public string? situacao { get; set; }
            public string? situacaoId { get; set; }
        }
    }
}
