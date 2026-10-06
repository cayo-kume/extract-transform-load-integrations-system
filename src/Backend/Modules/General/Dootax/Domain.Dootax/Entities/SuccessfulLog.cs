namespace Domain.Dootax.Entities
{
    public class SuccessfulLog
    {
        public Int32 id { get; set; }
        public string cnpj_emp { get; set; }
        public Int32 documento { get; set; }
        public string serie { get; set; }
        public string chave_nfe { get; set; }
        public DateTime dt_insert { get; set; }
    }
}
