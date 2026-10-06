using static Domain.Dootax.Entities.DootaxAttributes;

namespace Domain.Dootax.Entities
{
    public class XML
    {
        [DootaxAttribute(44, 1)]
        public string ChaveNfe { get; set; }

        [DootaxAttribute(30000, 2)]
        public string DsXml { get; set; }

        [DootaxAttribute(10, 3)]
        public string Documento { get; set; }

        [DootaxAttribute(16, 4)]
        public string CNPJCPF { get; set; }

        [DootaxAttribute(10, 5)]
        public string Serie { get; set; }

        [DootaxAttribute(12, 6)]
        public string NumeroPedido { get; set; }
    }
}
