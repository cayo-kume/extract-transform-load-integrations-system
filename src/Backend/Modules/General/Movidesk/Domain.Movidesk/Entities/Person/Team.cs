namespace Domain.Movidesk.Entities.Person
{
    public class Team
    {
        public string personId { get; private set; }
        public int id { get; private set; }
        public string team { get; private set; }

        public Team() { }

        public Team(string personId, int id, string team)
        {
            this.personId = personId;
            this.id = id;
            this.team = team;
        }
    }
}
