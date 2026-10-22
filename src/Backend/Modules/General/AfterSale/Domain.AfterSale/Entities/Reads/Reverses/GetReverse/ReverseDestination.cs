using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class ReverseDestination
    {
        public string? doc_company { get; private set; }

        public Int64 reverse_id { get; private set; }
        public Int64 id { get; private set; }

        public bool is_omnichannel { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime? address_updated_at { get; private set; }
        public DateTime? address_deleted_at { get; private set; }

        public string? ie_number { get; private set; }
        public string? cnpj { get; private set; }
        public string? display_name { get; private set; }
        public string? address_zip_code { get; private set; }
        public string? address_address { get; private set; }
        public string? address_number { get; private set; }
        public string? address_complement { get; private set; }
        public string? address_neighborhood { get; private set; }
        public string? address_city { get; private set; }
        public string? address_state { get; private set; }
        public string? address_country { get; private set; }
        public string? address_lat { get; private set; }
        public string? address_long { get; private set; }

        public ReverseDestination() { }

        public ReverseDestination(string doc_company, Dtos.Reads.Reverses.GetReverse.ReverseDestination reverseDestinationDto, Int64 reverse_id)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.reverse_id = reverse_id;
            id = reverse_id; // a API não devolve id para este bloco; 1:1 com a reversa

            is_omnichannel = ConverterValidatorsExtensions.ConvertToBooleanValidation(reverseDestinationDto.is_omnichannel);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            address_updated_at = NullableConverters.ToNullableDateTime(reverseDestinationDto.address.updated_at);
            address_deleted_at = NullableConverters.ToNullableDateTime(reverseDestinationDto.address.deleted_at);

            ie_number = reverseDestinationDto.ie_number?.Length > 50 ? reverseDestinationDto.ie_number?.Substring(0, 50) : reverseDestinationDto.ie_number;
            cnpj = reverseDestinationDto.cnpj?.Length > 14 ? reverseDestinationDto.cnpj?.Substring(0, 14) : reverseDestinationDto.cnpj;
            display_name = reverseDestinationDto.display_name?.Length > 250 ? reverseDestinationDto.display_name?.Substring(0, 250) : reverseDestinationDto.display_name;
            address_zip_code = reverseDestinationDto.address?.zip_code?.Length > 9 ? reverseDestinationDto.address?.zip_code?.Substring(0, 9) : reverseDestinationDto.address?.zip_code;
            address_address = reverseDestinationDto.address?.address?.Length > 250 ? reverseDestinationDto.address?.address?.Substring(0, 250) : reverseDestinationDto.address?.address;
            address_number = reverseDestinationDto.address?.number?.Length > 20 ? reverseDestinationDto.address?.number?.Substring(0, 20) : reverseDestinationDto.address?.number;
            address_complement = reverseDestinationDto.address?.complement?.Length > 50 ? reverseDestinationDto.address?.complement?.Substring(0, 50) : reverseDestinationDto.address?.complement;
            address_neighborhood = reverseDestinationDto.address?.neighborhood?.Length > 50 ? reverseDestinationDto.address?.neighborhood?.Substring(0, 50) : reverseDestinationDto.address?.neighborhood;
            address_city = reverseDestinationDto.address?.city?.Length > 40 ? reverseDestinationDto.address?.city?.Substring(0, 40) : reverseDestinationDto.address?.city;
            address_state = reverseDestinationDto.address?.state?.Length > 20 ? reverseDestinationDto.address?.state?.Substring(0, 20) : reverseDestinationDto.address?.state;
            address_country = reverseDestinationDto.address?.country?.Length > 80 ? reverseDestinationDto.address?.country?.Substring(0, 80) : reverseDestinationDto.address?.country;
            address_lat = reverseDestinationDto.address?.lat?.Length > 50 ? reverseDestinationDto.address?.lat?.Substring(0, 50) : reverseDestinationDto.address?.lat;
            address_long = reverseDestinationDto.address?._long?.Length > 50 ? reverseDestinationDto.address?._long?.Substring(0, 50) : reverseDestinationDto.address?._long;
        }
    }
}
