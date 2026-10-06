namespace Domain.Totvs.Entities.Product
{
    public class Grid
    {
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public string grid { get; private set; }

        public Grid() { }

        public Grid(string gridValue, int referenceId, int sequence)
        {
            this.referenceId = referenceId;
            this.sequence = sequence;
            grid = gridValue?.Length > 60 ? gridValue.Substring(0, 60) : gridValue;
        }
    }
}
