using Domain.Oto.Entities;
using Domain.Oto.Models;
using Newtonsoft.Json.Linq;

namespace Application.Oto.Factories
{
    public static class BuildObjects
    {
        public static JObject BuildCustomerObject(Client customer)
        {
            return new JObject
            {
                { "customer_id", $"{customer.Code}" },
                { "data_source", $"{customer.data_source}" },
                { "mobile_number", $"{customer.Phone}" },
                { "whatsapp_permission", true },
                { "first_name", $"{customer.Reason}" },
                { "create_date", $"{customer.created_date.ToString("yyyy-MM-dd")}" },
                { "city", $"{customer.City}" },
                { "document_number", $"{customer.Doc}" },
                { "email", $"{customer.Email}" },
                { "state", $"{customer.State}" },
                { "country", $"{customer.country}" },
                { "zipcode", $"{customer.ZipCode.Insert(5, "-")}" },
                { "address", $"{customer.Street}" },
                { "address_n", $"{customer.StreetNumber}" },
                { "address_complement", $"{customer.ComplementAddress}" },
                { "neighborhood", $"{customer.Neighborhood}" },
                { "gender", $"{customer.gender}" },
                { "date_birth", $"{customer.birthday_date.ToString("yyyy-MM-dd")}" },
                { "type_person", $"{customer.type}" },
                { "phone_number", $"{customer.Phone}" },
                { "custom_field_03", $"{customer.cashback_value}" }, //tem que ser no 3
                { "custom_field_04", $"{customer.store}" }, //tem que ser no 4
                { "custom_field_05", $"{customer.date.ToString("yyyy-MM-dd")}" } //tem que ser no 5
            };
        }

        public static JObject BuildStoreObject(Company company)
        {
            return new JObject
            {
                { "id", company.Code },
                { "name", company.Reason },
                { "name_otto", company.Reason },
                { "type", company.segment_company },
                { "country", company.country_company },
                { "state", company.State },
                { "city", company.City },
                { "neighborhood", company.Neighborhood },
                { "status", company.status_company }
            };
        }

        public static JObject BuildSellerObject(SaleRepresentative seller)
        {
            return new JObject
            {
                { "id", $"{seller.Code}" },
                { "store", $"{seller.company}" },
                { "name", $"{seller.Reason}" },
                { "name_otto", $"{seller.Reason}" }
            };
        }

        public static JObject BuildProductObject(Product product)
        {
            return new JObject
            {
                { "sku", $"{product.Sku}" },
                { "id", $"{product.Code}" },
                { "name", $"{product.Description}" },
                { "category", $"{product.type}" }
            };
        }

        public static JObject BuildOrderObject(Order order)
        {
            return new JObject
            {
                { "customer_id", $"{order.client?.Code}" },
                { "date", $"{order.date.ToString("yyyy-MM-dd")}" },
                { "id", $"{order.Number}" },
                { "store", $"{order.company?.Code}" },
                { "store_type", $"{order.store_type}" },
                { "canceled", $"{order.canceled}" },
                { "total", order.total }
            };
        }

        public static JObject BuildOrderItemObject(Product item, string date, string orderNumber, string codCompany, string sellerId)
        {
            return new JObject
            {
                { "date", $"{date}" },
                { "id", $"{orderNumber}" },
                { "store", $"{codCompany}" },
                { "sku", $"{item.Sku}" },
                { "seller_id", $"{sellerId}" },
                { "quantity", $"{item.quantity_product}" },
                { "price", $"{item.UnitaryValue}" },
                { "seq", $"{item.order}" }
            };
        }
    }
}
