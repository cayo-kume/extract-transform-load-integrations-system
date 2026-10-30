using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Similars
{
    /// <summary>
    /// produto com os seus similares, tabela totvs.TotvsProductsSimilar. Sem data de alteração útil na resposta (lastChageDate vem
    /// 0001-01-01): json_hash (SHA-256 do JSON do produto) diz se ele mudou (T9 do src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md), e
    /// changeWindowEnd (o fim da janela que o trouxe) é de onde a próxima leitura começa. A lista de
    /// produtos da mesma grade (similarGroupProductList) não é gravada: sai de TotvsSkusColorsProducts pela referência (§5.2)
    /// </summary>
    public class ProductSimilar
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int productCode { get; private set; }
        public int? groupSequenceCode { get; private set; }
        public DateTime? changeWindowEnd { get; private set; }
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<ProductSimilarItem> items { get; private set; } = [];

        public ProductSimilar() { }

        public ProductSimilar(string doc_company, DateTime integrated_or_updated_at, DateTime changeWindowEnd, string json_hash, Dtos.Reads.Products.Similars.ProductSimilar dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;
            this.changeWindowEnd = changeWindowEnd;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            groupSequenceCode = NullableConverters.ToNullableInt32(dto.groupSequenceCode);

            // listType: Product (similarProductList) ou ReferenceGroup (similarRefereceGroupProductList); repetidos violariam a PK
            items.AddRange((dto.similarProductList ?? []).Select(x => new ProductSimilarItem(doc_company, integrated_or_updated_at, productCode, "Product", x))
                .Concat((dto.similarRefereceGroupProductList ?? []).Select(x => new ProductSimilarItem(doc_company, integrated_or_updated_at, productCode, "ReferenceGroup", x)))
                .Where(x => x.similarProductCode != 0)
                .GroupBy(x => (x.listType, x.similarProductCode))
                .Select(g => g.Last()));
        }
    }
}
