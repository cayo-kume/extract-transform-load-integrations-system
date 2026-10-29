using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// etiqueta do pedido, tabela linxcommerce.LinxCommerceOrdersTags
    /// </summary>
    public class OrderTag
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public int TagID { get; private set; }
        public string Alias { get; private set; } = String.Empty;
        public string Name { get; private set; } = String.Empty;
        public bool? IsSystem { get; private set; }
        public bool? IsDeleted { get; private set; }

        public OrderTag() { }

        public OrderTag(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderTag dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            TagID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.TagID);
            Alias = ConverterValidatorsExtensions.StringLengthValidation(dto.Alias, 100);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            IsSystem = NullableConverters.ToNullableBoolean(dto.IsSystem);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
        }
    }
}
