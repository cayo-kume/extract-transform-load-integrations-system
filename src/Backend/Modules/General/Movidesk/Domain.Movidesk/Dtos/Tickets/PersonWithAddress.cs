namespace Domain.Movidesk.Dtos.Tickets
{
    public class PersonWithAddress : Person
    {
        public string address { get; set; }
        public string complement { get; set; }
        public string cep { get; set; }
        public string city { get; set; }
        public string bairro { get; set; }
        public string number { get; set; }
        public string reference { get; set; }
    }
}
