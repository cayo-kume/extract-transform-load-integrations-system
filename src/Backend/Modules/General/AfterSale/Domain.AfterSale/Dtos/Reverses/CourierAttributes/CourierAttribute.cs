using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reverses.CourierAttributes
{
    public class CourierAttribute
    {
        public Reverse reverse { get; init; }
        public List<Field> fields { get; init; }
        public string fill_form_from_file { get; init; }
    }
}
