using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Oto.Entities
{
    public class SentRecordLog
    {
        public Guid identificador { get; private set; }
        public string branch { get; private set; }
        public string documento { get; private set; }
        public string tipo { get; private set; }
        public DateTime data_envio { get; private set; }
        public string payload { get; private set; }

        public SentRecordLog() { }

        public SentRecordLog(string branch, string documento, string tipo, string payload)
        {
            this.branch = branch;
            this.documento = documento;
            this.tipo = tipo;
            this.payload = payload;
            data_envio = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        }

        public SentRecordLog(Guid identificador, string branch, string documento, string tipo, string payload)
        {
            this.identificador = identificador;
            this.branch = branch;
            this.documento = documento;
            this.tipo = tipo;
            this.payload = payload;
            data_envio = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        }
    }
}
