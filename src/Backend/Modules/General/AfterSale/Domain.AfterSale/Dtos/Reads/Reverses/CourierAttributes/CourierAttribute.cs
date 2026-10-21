using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes
{
    public class CourierAttribute
    {
        public Reverse reverse { get; init; }
        public List<Field> fields { get; init; }

        // pode vir null ou como objeto ({ "fields": { "<caminho no XML>": "<id do campo>" } }), conforme a transportadora
        public JToken? fill_form_from_file { get; init; }
    }
}
