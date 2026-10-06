using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.TotalExpress.Entities
{
    public class Error
    {
        /// <summary>
        /// auto increment property created to be the primary key of the migration
        /// </summary>
        [SkipProperty]
        public Int64 id { get; set; }

        public string pedido { get; set; }
        public string erro { get; set; }

        [NotMapped]
        [SkipProperty]
        /// <summary>
        /// 
        ///</summary>
        public Dictionary<string?, string> Responses { get; set; } = new Dictionary<string?, string>();

        public Error() { }

        public Error(string pedido, string erro)
        {
            this.pedido = pedido;
            this.erro = erro;
            this.Responses.Add(pedido, erro);
        }
    }
}
