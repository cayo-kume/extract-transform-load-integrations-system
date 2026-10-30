using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// região de atuação (GET seller/v2/operational-area), tabela totvs.TotvsOperationalAreas, com os estados, CEPs e cidades
    /// das outras três rotas. Sem filtro de data a API não preenche o maxChangeFilterDate: json_hash (SHA-256 da região agrupada
    /// com as filhas) diz se ela mudou (T9 do src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class OperationalArea
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int operationalAreaCode { get; private set; }
        public string operationalAreaName { get; private set; } = String.Empty;
        public string operationalAreaType { get; private set; } = String.Empty;
        public int? operationalAreaGroupCode { get; private set; }
        public string operationalAreaGroupDescription { get; private set; } = String.Empty;
        public int? numberOfDays { get; private set; }
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<OperationalAreaState> states { get; private set; } = [];

        [SkipProperty]
        public List<OperationalAreaCep> ceps { get; private set; } = [];

        [SkipProperty]
        public List<OperationalAreaCity> cities { get; private set; } = [];

        public OperationalArea() { }

        public OperationalArea(
            string doc_company,
            DateTime integrated_or_updated_at,
            string json_hash,
            Dtos.Reads.Sellers.OperationalArea dto,
            IEnumerable<Dtos.Reads.Sellers.OperationalAreaState> statesDto,
            IEnumerable<Dtos.Reads.Sellers.OperationalAreaCep> cepsDto,
            IEnumerable<Dtos.Reads.Sellers.OperationalAreaCity> citiesDto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            operationalAreaCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.operationalAreaCode);
            operationalAreaName = ConverterValidatorsExtensions.StringLengthValidation(dto.operationalAreaName, 100);
            operationalAreaType = ConverterValidatorsExtensions.StringLengthValidation(dto.operationalAreaType, 30);
            operationalAreaGroupCode = NullableConverters.ToNullableInt32(dto.operationalAreaGroupCode);
            operationalAreaGroupDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.operationalAreaGroupDescription, 100);
            numberOfDays = NullableConverters.ToNullableInt32(dto.numberOfDays);

            // repetidos violariam a PK
            states.AddRange(statesDto.Select(x => new OperationalAreaState(doc_company, integrated_or_updated_at, operationalAreaCode, x)).GroupBy(x => x.stateCode).Select(g => g.Last()));
            ceps.AddRange(cepsDto.Select(x => new OperationalAreaCep(doc_company, integrated_or_updated_at, operationalAreaCode, x)).GroupBy(x => x.cep.ToUpperInvariant()).Select(g => g.Last()));
            cities.AddRange(citiesDto.Select(x => new OperationalAreaCity(doc_company, integrated_or_updated_at, operationalAreaCode, x)).GroupBy(x => x.cityCode).Select(g => g.Last()));
        }
    }
}
