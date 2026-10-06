namespace Domain.Dootax.Entities
{
    public class DootaxAttributes
    {
        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        public class DootaxAttribute : System.Attribute
        {
            public int FieldSize { get; set; }
            public int FieldPosition { get; set; }

            public bool FieldIsNumber { get; set; }

            public bool FieldIsDecimal { get; set; }

            public bool FieldAlignRight { get; set; }
            public DootaxAttribute(int Size, int Position, bool IsNumber = false, bool IsDecimal = false, bool AlignRight = false)
            {
                FieldSize = Size;
                FieldPosition = Position;
                FieldIsNumber = IsNumber;
                FieldIsDecimal = IsDecimal;
                FieldAlignRight = AlignRight;
            }
        }
    }
}
