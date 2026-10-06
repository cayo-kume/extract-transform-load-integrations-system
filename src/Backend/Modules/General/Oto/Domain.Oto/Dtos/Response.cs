using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Oto.Dtos
{
    public class Response
    {
        public bool success { get; set; }
        public Guid requestId { get; set; }
    }
}
