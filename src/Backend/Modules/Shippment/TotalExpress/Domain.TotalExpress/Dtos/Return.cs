namespace Domain.TotalExpress.Dtos
{
    public class Return
    {

        public class Rootobject
        {
            public Retorno retorno { get; set; }
        }

        public class Retorno
        {
            public Encomenda[] encomendas { get; set; }
        }

        public class Encomenda
        {
            public string pedido { get; set; }
            public string clienteCodigo { get; set; }
            public string tipoServico { get; set; }
            public string data { get; set; }
            public string hora { get; set; }
            public Volume[] volumes { get; set; }
            public Documentofiscal[] documentoFiscal { get; set; }
        }

        public class Volume
        {
            public string awb { get; set; }
            public string rota { get; set; }
            public string codigoBarras { get; set; }
        }

        public class Documentofiscal
        {
            public string serie { get; set; }
            public string numero { get; set; }
        }

    }
}
