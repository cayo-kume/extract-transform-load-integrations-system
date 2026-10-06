namespace Domain.Shared.Entities.Base
{
    public abstract class ProductBase
    {
        public int cod_product { get; init; }

        public double quantity_product { get; init; }

        public string? sku_product { get; init; }

        public string? description_product { get; init; }

        public string? cod_ean_product { get; init; }

        public string? unitary_value_product { get; init; }

        public string? amount_product { get; init; }

        public string? shipping_value_product { get; init; }

        public int CompareTo(ProductBase other)
        {
            if (cod_product > other.cod_product)
                return 1;
            else if (cod_product < other.cod_product)
                return -1;
            else
                return 0;
        }
    }
}
