namespace Domain.Totvs.Entities.Product
{
    public class Observation
    {
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; }

        public Observation() { }

        public Observation(int referenceId, int sequence, Dtos.Product.Observation observationDto)
        {
            this.referenceId = referenceId;
            this.sequence = sequence;
            observation = observationDto.observation?.Length > 300 ? observationDto.observation.Substring(0, 300) : observationDto.observation;
        }
    }
}
