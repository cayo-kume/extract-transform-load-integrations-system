using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class DestinationData
    {
        public string? doc_company { get; private set; }

        public Int64 reverse_id { get; private set; }
        public Int64 id { get; private set; }
        public Int64 address_id { get; private set; }

        public int type_id { get; private set; }

        public bool return_to_seller { get; private set; }
        public bool was_manually_changed { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime address_deleted_at { get; private set; }
        public DateTime address_updated_at { get; private set; }

        public string? name { get; private set; }
        public string? type { get; private set; }
        public string? address_lat { get; private set; }
        public string? address_city { get; private set; }
        public string? address_long { get; private set; }
        public string? address_state { get; private set; }
        public string? address_number { get; private set; }
        public string? address_address { get; private set; }
        public string? address_country { get; private set; }
        public string? address_zip_code { get; private set; }
        public string? address_complement { get; private set; }
        public string? address_neighborhood { get; private set; }
        public DateTime updated_at { get; private set; }
        public string? seller_info { get; private set; }
        public string? label { get; private set; }

        public DestinationData() { }

        public DestinationData(string doc_company, Dtos.Reads.Reverses.GetReverse.DestinationData destinationDataDto, Int64 reverse_id)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.reverse_id = reverse_id;
            id = reverse_id; // a API não devolve id para este bloco; 1:1 com a reversa
            address_id = ConverterValidatorsExtensions.ConvertToInt64Validation(destinationDataDto.address.id);

            type_id = ConverterValidatorsExtensions.ConvertToInt32Validation(destinationDataDto.type_id);

            return_to_seller = ConverterValidatorsExtensions.ConvertToBooleanValidation(destinationDataDto.return_to_seller);
            was_manually_changed = ConverterValidatorsExtensions.ConvertToBooleanValidation(destinationDataDto.was_manually_changed);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            address_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(destinationDataDto.address.deleted_at);
            address_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(destinationDataDto.address.updated_at);

            name = destinationDataDto.name?.Length > 250 ? destinationDataDto.name?.Substring(0, 250) : destinationDataDto.name;
            type = destinationDataDto.type?.Length > 50 ? destinationDataDto.type?.Substring(0, 50) : destinationDataDto.type;
            address_lat = destinationDataDto.address?.lat?.Length > 50 ? destinationDataDto.address?.lat?.Substring(0, 50) : destinationDataDto.address?.lat;
            address_city = destinationDataDto.address?.city?.Length > 40 ? destinationDataDto.address?.city?.Substring(0, 40) : destinationDataDto.address?.city;
            address_long = destinationDataDto.address?._long?.Length > 50 ? destinationDataDto.address?._long?.Substring(0, 50) : destinationDataDto.address?._long;
            address_state = destinationDataDto.address?.state?.Length > 20 ? destinationDataDto.address?.state?.Substring(0, 20) : destinationDataDto.address?.state;
            address_number = destinationDataDto.address?.number?.Length > 20 ? destinationDataDto.address?.number?.Substring(0, 20) : destinationDataDto.address?.number;
            address_address = destinationDataDto.address?.address?.Length > 250 ? destinationDataDto.address?.address?.Substring(0, 250) : destinationDataDto.address?.address;
            address_country = destinationDataDto.address?.country?.Length > 80 ? destinationDataDto.address?.country?.Substring(0, 80) : destinationDataDto.address?.country;
            address_zip_code = destinationDataDto.address?.zip_code?.Length > 9 ? destinationDataDto.address?.zip_code?.Substring(0, 9) : destinationDataDto.address?.zip_code;
            address_complement = destinationDataDto.address?.complement?.Length > 50 ? destinationDataDto.address?.complement?.Substring(0, 50) : destinationDataDto.address?.complement;
            address_neighborhood = destinationDataDto.address?.neighborhood?.Length > 50 ? destinationDataDto.address?.neighborhood?.Substring(0, 50) : destinationDataDto.address?.neighborhood;
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(destinationDataDto.updated_at);
            seller_info = destinationDataDto.seller_info?.Length > 50 ? destinationDataDto.seller_info?.Substring(0, 50) : destinationDataDto.seller_info;
            label = destinationDataDto.label?.Length > 300 ? destinationDataDto.label?.Substring(0, 300) : destinationDataDto.label;
        }
    }
}
