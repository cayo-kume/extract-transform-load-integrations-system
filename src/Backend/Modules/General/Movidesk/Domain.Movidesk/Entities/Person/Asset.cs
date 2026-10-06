namespace Domain.Movidesk.Entities.Person
{
    public class Asset
    {
        public string personId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string label { get; private set; }

        public Asset() { }

        public Asset(string personId, Dtos.Person.Asset assetDto)
        {
            this.personId = personId;

            id = assetDto.id;
            name = assetDto.name;
            label = assetDto.label;
        }
    }
}
