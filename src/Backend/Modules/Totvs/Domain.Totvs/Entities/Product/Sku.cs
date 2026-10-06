using Domain.Shared.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Domain.Totvs.Entities.Product
{
    public class Sku
    {
        public string referenceCode { get; private set; }
        public int referenceId { get; private set; }
        public string lastReferenceCode { get; private set; }
        public DateTime insertDate { get; private set; }
        public DateTime maxChangeFilterDate { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string descriptive { get; private set; }
        public int gridCode { get; private set; }
        public decimal weight { get; private set; }
        public decimal height { get; private set; }
        public decimal width { get; private set; }
        public decimal length { get; private set; }
        public decimal packWeight { get; private set; }
        public decimal packHeight { get; private set; }
        public decimal packWidth { get; private set; }
        public decimal packLength { get; private set; }

        [SkipProperty]
        public List<Grid> grids { get; private set; } = [];

        [SkipProperty]
        public List<Observation> observations { get; private set; } = [];

        [SkipProperty]
        public List<Detail> details { get; private set; } = [];

        [SkipProperty]
        public List<Composition> compositions { get; private set; } = [];

        [SkipProperty]
        public List<ReferenceCategory> referenceCategories { get; private set; } = [];

        [SkipProperty]
        public List<Color> colors { get; private set; } = [];

        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = [];

        public Sku() { }

        public Sku(Domain.Totvs.Dtos.Product.Sku skuDto)
        {
            referenceCode = skuDto.ReferenceCode?.Length > 60 ? skuDto.ReferenceCode.Substring(0, 60) : skuDto.ReferenceCode;
            referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation(skuDto.referenceId);
            lastReferenceCode = skuDto.lastReferenceCode?.Length > 60 ? skuDto.lastReferenceCode.Substring(0, 60) : skuDto.lastReferenceCode;
            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(skuDto.insertDate);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(skuDto.maxChangeFilterDate);
            name = skuDto.name?.Length > 250 ? skuDto.name.Substring(0, 250) : skuDto.name;
            description = skuDto.description?.Length > 250 ? skuDto.description.Substring(0, 250) : skuDto.description;
            descriptive = skuDto.descriptive?.Length > 250 ? skuDto.descriptive.Substring(0, 250) : skuDto.descriptive;
            gridCode = ConverterValidatorsExtensions.ConvertToInt32Validation(skuDto.gridCode);
            weight = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.weight);
            height = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.height);
            width = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.width);
            length = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.length);
            packWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.packWeight);
            packHeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.packHeight);
            packWidth = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.packWidth);
            packLength = ConverterValidatorsExtensions.ConvertToDecimalValidation(skuDto.packLength);

            Responses.Add(skuDto.referenceId, JsonConvert.SerializeObject(skuDto));

            for (int i = 0; i < skuDto.grid.Count(); i++)
            {
                grids.Add(new Grid(skuDto.grid[i], referenceId, i + 1));
            }

            for (int i = 0; i < skuDto.observations.Count(); i++)
            {
                observations.Add(new Observation(referenceId, i + 1, skuDto.observations[i]));
            }

            skuDto.details?
                .GroupBy(d => d.typeCode)
                .Select(g => g.First())
                .ToList()
                .ForEach(detail =>
                    details.Add(new Detail(referenceId, detail)
                ));

            skuDto.composition?.ForEach(composition =>
                compositions.Add(new Composition(referenceId, composition)
            ));

            skuDto.referenceCategories?.ForEach(referenceCategorie =>
                referenceCategories.Add(new ReferenceCategory(referenceId, referenceCategorie)
            ));

            skuDto.colors?.ForEach(color =>
                colors.Add(new Color(referenceId, color)
            ));
        }
    }
}
