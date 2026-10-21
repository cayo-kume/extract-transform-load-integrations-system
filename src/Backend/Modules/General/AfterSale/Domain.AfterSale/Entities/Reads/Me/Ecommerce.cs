using Domain.Shared.Extensions;

namespace Domain.AfterSale.Entities.Reads.Me
{
    public class Ecommerce
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int address_id { get; private set; }

        public Guid uuid { get; private set; }

        public string? hotjar_id { get; private set; }
        public string? oauth_client_id { get; private set; }
        public string? provider_id { get; private set; }
        public string? brand_id { get; private set; }
        public string? ecommerce_group_id { get; private set; }
        public string? hubspot_code { get; private set; }
        public string? phone { get; private set; }
        public string? registration_origin { get; private set; }
        public string? bank_agency_max_length { get; private set; }
        public string? correios_checklist_default_type { get; private set; }
        public string? company_name { get; private set; }
        public string? trade_name { get; private set; }
        public string? display_name { get; private set; }
        public string? url { get; private set; }
        public string? email { get; private set; }
        public string? document { get; private set; }
        public string? segment { get; private set; }
        public string? app_name { get; private set; }
        public string? theme { get; private set; }
        public string? logo { get; private set; }
        public string? terms { get; private set; }
        public string? limit_value_collect { get; private set; }

        public string? field_order_id_label { get; private set; }
        public string? field_order_id_placeholder { get; private set; }
        public string? field_product_comment_label { get; private set; }
        public string? field_product_comment_placeholder { get; private set; }
        public string? field_confirmation_label_email { get; private set; }
        public string? field_confirmation_placeholder_email { get; private set; }
        public string? field_confirmation_label_document { get; private set; }
        public string? field_confirmation_placeholder_document { get; private set; }
        public string? field_confirmation_type { get; private set; }

        public string? retention_method { get; private set; }
        public string? retention_layout_version { get; private set; }
        public string? request_evaluation_method { get; private set; }

        public string? address_zip_code { get; private set; }
        public string? address_address { get; private set; }
        public string? address_complement { get; private set; }
        public string? address_number { get; private set; }
        public string? address_neighborhood { get; private set; }
        public string? address_city { get; private set; }
        public string? address_state { get; private set; }
        public string? address_country { get; private set; }
        public string? address_lat { get; private set; }
        public string? address_long { get; private set; }

        public string? localization_currency { get; private set; }
        public string? localization_timezone { get; private set; }
        public string? localization_language { get; private set; }

        public string? shortcodes_ecommerce_display_name { get; private set; }
        public string? shortcodes_ecommerce_logo_email_url { get; private set; }
        public string? shortcodes_ecommerce_logo_url { get; private set; }
        public string? shortcodes_ecommerce_url { get; private set; }
        public string? shortcodes_ecommerce_phone { get; private set; }
        public string? shortcodes_ecommerce_name { get; private set; }
        public string? shortcodes_app_name { get; private set; }

        public string? stylesheet_text_primary { get; private set; }
        public string? stylesheet_table_primary { get; private set; }
        public string? stylesheet_background_primary { get; private set; }
        public string? stylesheet_button_primary { get; private set; }
        public string? stylesheet_button_text { get; private set; }
        public string? stylesheet_button_borderradius { get; private set; }
        public string? stylesheet_input_text { get; private set; }
        public string? stylesheet_input_background { get; private set; }
        public string? stylesheet_input_border { get; private set; }
        public string? stylesheet_input_borderradius { get; private set; }
        public string? stylesheet_paragraph_primary { get; private set; }
        public string? stylesheet_label_primary { get; private set; }
        public string? stylesheet_whitebox_primary { get; private set; }
        public string? stylesheet_whitebox_boxshadow { get; private set; }

        public string? reverse_actions_customization_labels_online_exchange { get; private set; }
        public string? reverse_actions_customization_labels_online_return { get; private set; }
        public string? reverse_actions_customization_labels_offline_exchange { get; private set; }
        public string? reverse_actions_customization_labels_offline_return { get; private set; }

        [SkipProperty]
        public string response { get; private set; }

        public bool is_active { get; private set; }
        public bool maintenance_mode_global { get; private set; }
        public bool is_test { get; private set; }
        public bool deny_search_index { get; private set; }
        public bool collect_enabled { get; private set; }
        public bool has_ecommerce_integration { get; private set; }
        public bool has_info_by_customer_enabled { get; private set; }
        public bool hotjar_enabled { get; private set; }
        public bool retention_free_shipping { get; private set; }
        public bool show_quick_selection { get; private set; }
        public bool maintenance_mode { get; private set; }
        public bool enable_price_visibility { get; private set; }
        public bool shipping_address_change_enabled { get; private set; }
        public bool request_evaluation_enabled { get; private set; }
        public bool enable_transportation_priority_by_preferences { get; private set; }
        public bool force_courier_show_when_postage_block_by_pickup_in_use { get; private set; }
        public bool refund_pix_enabled { get; private set; }
        public bool allow_refund_for_third_parties_by_pix { get; private set; }
        public bool could_be_retained { get; private set; }
        public bool cashback_allow_third_party { get; private set; }
        public bool request_pix_key_by_customer_interface { get; private set; }
        public bool correios_checklist_enabled { get; private set; }
        public bool request_customer_invoice_xml_enabled { get; private set; }
        public bool split_reverses_by_seller { get; private set; }
        public bool group_products_with_unregistered_sellers { get; private set; }
        public bool care_plus_enabled { get; private set; }
        public bool hide_courier_enabled { get; private set; }
        public bool show_product_images { get; private set; }

        public bool reverse_actions_customization_online_exchange { get; private set; }
        public bool reverse_actions_customization_online_return { get; private set; }
        public bool reverse_actions_customization_offline_exchange { get; private set; }
        public bool reverse_actions_customization_offline_return { get; private set; }

        public DateTime last_order_report_date { get; private set; }
        public DateTime address_updated_at { get; private set; }
        public DateTime address_deleted_at { get; private set; }

        [SkipProperty]
        public MessagePage messages_pages { get; private set; }

        [SkipProperty]
        public MessageMail messages_mails { get; private set; }

        [SkipProperty]
        public List<Reason> reasons { get; private set; } = [];

        [SkipProperty]
        public List<StringListValue> ecommerce_string_values { get; private set; } = [];

        //Up to the date this integration was developed, the JSON parsed for object creation returned empty arrays for the following properties.
        //If there is future data to be filled, change the property type and uncomment the code.
        //public List<object> transportation_modal_priority_order { get; init; }
        //public List<object> correios_checklist_eletronic_categories { get; init; }
        //public List<object> correios_checklist_phone_categories { get; init; }
        //public List<object> correios_checklist_content_categories { get; init; }
        //public List<object> modules_fiscal { get; init; }
        //public List<object> modules_insurance { get; init; }
        //public List<object> modules_logistic { get; init; }
        //public List<object> modules_order_cancellation { get; init; }
        //public List<object> modules_whitelabel { get; init; }
        //public List<object> modules_security { get; init; }

        public Ecommerce() { }

        public Ecommerce(Dtos.Reads.Me.Ecommerce ecommerceDto, string response)
        {
            this.integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(ecommerceDto.id);
            address_id = ConverterValidatorsExtensions.ConvertToInt32Validation(ecommerceDto.address_id);

            uuid = ConverterValidatorsExtensions.ConvertToGuidValidation(ecommerceDto.uuid);

            hotjar_id = ecommerceDto.hotjar_id?.Length > 50 ? ecommerceDto.hotjar_id?.Substring(0, 50) : ecommerceDto.hotjar_id;
            provider_id = ecommerceDto.provider_id?.Length > 50 ? ecommerceDto.provider_id?.Substring(0, 50) : ecommerceDto.provider_id;
            oauth_client_id = ecommerceDto.oauth_client_id?.Length > 50 ? ecommerceDto.oauth_client_id?.Substring(0, 50) : ecommerceDto.oauth_client_id;
            brand_id = ecommerceDto.brand_id?.Length > 50 ? ecommerceDto.brand_id?.Substring(0, 50) : ecommerceDto.brand_id;
            ecommerce_group_id = ecommerceDto.ecommerce_group_id?.Length > 50 ? ecommerceDto.ecommerce_group_id?.Substring(0, 50) : ecommerceDto.ecommerce_group_id;
            hubspot_code = ecommerceDto.hubspot_code?.Length > 50 ? ecommerceDto.hubspot_code?.Substring(0, 50) : ecommerceDto.hubspot_code;
            phone = ecommerceDto.phone?.Length > 20 ? ecommerceDto.phone?.Substring(0, 20) : ecommerceDto.phone;
            registration_origin = ecommerceDto.registration_origin?.Length > 50 ? ecommerceDto.registration_origin?.Substring(0, 50) : ecommerceDto.registration_origin;
            bank_agency_max_length = ecommerceDto.bank_agency_max_length?.Length > 50 ? ecommerceDto.bank_agency_max_length?.Substring(0, 50) : ecommerceDto.bank_agency_max_length;
            correios_checklist_default_type = ecommerceDto.correios_checklist_default_type?.Length > 50 ? ecommerceDto.correios_checklist_default_type?.Substring(0, 50) : ecommerceDto.correios_checklist_default_type;
            company_name = ecommerceDto.company_name?.Length > 120 ? ecommerceDto.company_name?.Substring(0, 120) : ecommerceDto.company_name;
            trade_name = ecommerceDto.trade_name?.Length > 120 ? ecommerceDto.trade_name?.Substring(0, 120) : ecommerceDto.trade_name;
            display_name = ecommerceDto.display_name?.Length > 612 ? ecommerceDto.display_name?.Substring(0, 612) : ecommerceDto.display_name;
            url = ecommerceDto.url?.Length > 300 ? ecommerceDto.url?.Substring(0, 300) : ecommerceDto.url;
            email = ecommerceDto.email?.Length > 300 ? ecommerceDto.email?.Substring(0, 300) : ecommerceDto.email;
            document = ecommerceDto.document?.Length > 14 ? ecommerceDto.document?.Substring(0, 14) : ecommerceDto.document;
            segment = ecommerceDto.segment?.Length > 50 ? ecommerceDto.segment?.Substring(0, 50) : ecommerceDto.segment;
            app_name = ecommerceDto.app_name?.Length > 50 ? ecommerceDto.app_name?.Substring(0, 50) : ecommerceDto.app_name;
            theme = ecommerceDto.theme?.Length > 50 ? ecommerceDto.theme?.Substring(0, 50) : ecommerceDto.theme;
            logo = ecommerceDto.logo?.Length > 300 ? ecommerceDto.logo?.Substring(0, 300) : ecommerceDto.logo;
            terms = ecommerceDto.terms?.Length > 300 ? ecommerceDto.terms?.Substring(0, 300) : ecommerceDto.terms;
            limit_value_collect = ecommerceDto.limit_value_collect?.Length > 50 ? ecommerceDto.limit_value_collect?.Substring(0, 50) : ecommerceDto.limit_value_collect;

            field_order_id_label = ecommerceDto.field_order_id_label?.Length > 50 ? ecommerceDto.field_order_id_label?.Substring(0, 50) : ecommerceDto.field_order_id_label;
            field_order_id_placeholder = ecommerceDto.field_order_id_placeholder?.Length > 50 ? ecommerceDto.field_order_id_placeholder?.Substring(0, 50) : ecommerceDto.field_order_id_placeholder;
            field_product_comment_label = ecommerceDto.field_product_comment_label?.Length > 50 ? ecommerceDto.field_product_comment_label?.Substring(0, 50) : ecommerceDto.field_product_comment_label;
            field_product_comment_placeholder = ecommerceDto.field_product_comment_placeholder?.Length > 50 ? ecommerceDto.field_product_comment_placeholder?.Substring(0, 50) : ecommerceDto.field_product_comment_placeholder;
            field_confirmation_label_email = ecommerceDto.field_confirmation_label_email?.Length > 50 ? ecommerceDto.field_confirmation_label_email?.Substring(0, 50) : ecommerceDto.field_confirmation_label_email;
            field_confirmation_placeholder_email = ecommerceDto.field_confirmation_placeholder_email?.Length > 50 ? ecommerceDto.field_confirmation_placeholder_email?.Substring(0, 50) : ecommerceDto.field_confirmation_placeholder_email;
            field_confirmation_label_document = ecommerceDto.field_confirmation_label_document?.Length > 50 ? ecommerceDto.field_confirmation_label_document?.Substring(0, 50) : ecommerceDto.field_confirmation_label_document;
            field_confirmation_placeholder_document = ecommerceDto.field_confirmation_placeholder_document?.Length > 50 ? ecommerceDto.field_confirmation_placeholder_document?.Substring(0, 50) : ecommerceDto.field_confirmation_placeholder_document;
            field_confirmation_type = ecommerceDto.field_confirmation_type?.Length > 50 ? ecommerceDto.field_confirmation_type?.Substring(0, 50) : ecommerceDto.field_confirmation_type;

            retention_method = ecommerceDto.retention_method?.Length > 50 ? ecommerceDto.retention_method.Substring(0, 50) : ecommerceDto.retention_method;
            retention_layout_version = ecommerceDto.retention_layout_version?.Length > 50 ? ecommerceDto.retention_layout_version.Substring(0, 50) : ecommerceDto.retention_layout_version;
            request_evaluation_method = ecommerceDto.request_evaluation_method?.Length > 50 ? ecommerceDto.request_evaluation_method.Substring(0, 50) : ecommerceDto.request_evaluation_method;

            address_zip_code = ecommerceDto.address?.zip_code?.Length > 9 ? ecommerceDto.address?.zip_code?.Substring(0, 9) : ecommerceDto.address?.zip_code;
            address_address = ecommerceDto.address?.address?.Length > 250 ? ecommerceDto.address?.address?.Substring(0, 250) : ecommerceDto.address?.address;
            address_complement = ecommerceDto.address?.complement?.Length > 50 ? ecommerceDto.address?.complement?.Substring(0, 50) : ecommerceDto.address?.complement;
            address_number = ecommerceDto.address?.number?.Length > 20 ? ecommerceDto.address?.number?.Substring(0, 20) : ecommerceDto.address?.number;
            address_neighborhood = ecommerceDto.address?.neighborhood?.Length > 50 ? ecommerceDto.address?.neighborhood?.Substring(0, 50) : ecommerceDto.address?.neighborhood;
            address_city = ecommerceDto.address?.city?.Length > 40 ? ecommerceDto.address?.city?.Substring(0, 40) : ecommerceDto.address?.city;
            address_state = ecommerceDto.address?.state?.Length > 50 ? ecommerceDto.address?.state?.Substring(0, 50) : ecommerceDto.address?.state;
            address_country = ecommerceDto.address?.country?.Length > 50 ? ecommerceDto.address?.country?.Substring(0, 50) : ecommerceDto.address?.country;
            address_lat = ecommerceDto.address?.lat?.Length > 50 ? ecommerceDto.address?.lat?.Substring(0, 50) : ecommerceDto.address?.lat;
            address_long = ecommerceDto.address?.@long?.Length > 50 ? ecommerceDto.address?.@long?.Substring(0, 50) : ecommerceDto.address?.@long;

            reverse_actions_customization_labels_online_exchange = ecommerceDto.reverse_actions_customization_labels?.online?.exchange?.Length > 50 ? ecommerceDto.reverse_actions_customization_labels?.online?.exchange?.Substring(0, 50) : ecommerceDto.reverse_actions_customization_labels?.online?.exchange;
            reverse_actions_customization_labels_online_return = ecommerceDto.reverse_actions_customization_labels?.online?.@return?.Length > 50 ? ecommerceDto.reverse_actions_customization_labels?.online?.@return?.Substring(0, 50) : ecommerceDto.reverse_actions_customization_labels?.online?.@return;
            reverse_actions_customization_labels_offline_exchange = ecommerceDto.reverse_actions_customization_labels?.offline?.exchange?.Length > 50 ? ecommerceDto.reverse_actions_customization_labels?.offline?.exchange?.Substring(0, 50) : ecommerceDto.reverse_actions_customization_labels?.offline?.exchange;
            reverse_actions_customization_labels_offline_return = ecommerceDto.reverse_actions_customization_labels?.offline?.@return?.Length > 50 ? ecommerceDto.reverse_actions_customization_labels?.offline?.@return?.Substring(0, 50) : ecommerceDto.reverse_actions_customization_labels?.offline?.@return;

            localization_currency = ecommerceDto.localization?.currency?.Length > 50 ? ecommerceDto.localization?.currency?.Substring(0, 50) : ecommerceDto.localization?.currency;
            localization_timezone = ecommerceDto.localization?.timezone?.Length > 50 ? ecommerceDto.localization?.timezone?.Substring(0, 50) : ecommerceDto.localization?.timezone;
            localization_language = ecommerceDto.localization?.language?.Length > 50 ? ecommerceDto.localization?.language?.Substring(0, 50) : ecommerceDto.localization?.language;

            shortcodes_ecommerce_display_name = ecommerceDto.shortcodes?.ecommerce_display_name?.Length > 50 ? ecommerceDto.shortcodes?.ecommerce_display_name?.Substring(0, 50) : ecommerceDto.shortcodes?.ecommerce_display_name;
            shortcodes_ecommerce_logo_email_url = ecommerceDto.shortcodes?.ecommerce_logo_email_url?.Length > 300 ? ecommerceDto.shortcodes?.ecommerce_logo_email_url?.Substring(0, 300) : ecommerceDto.shortcodes?.ecommerce_logo_email_url;
            shortcodes_ecommerce_logo_url = ecommerceDto.shortcodes?.ecommerce_logo_url?.Length > 300 ? ecommerceDto.shortcodes?.ecommerce_logo_url?.Substring(0, 300) : ecommerceDto.shortcodes?.ecommerce_logo_url;
            shortcodes_ecommerce_url = ecommerceDto.shortcodes?.ecommerce_url?.Length > 300 ? ecommerceDto.shortcodes?.ecommerce_url?.Substring(0, 300) : ecommerceDto.shortcodes?.ecommerce_url;
            shortcodes_ecommerce_phone = ecommerceDto.shortcodes?.ecommerce_phone?.Length > 50 ? ecommerceDto.shortcodes?.ecommerce_phone?.Substring(0, 50) : ecommerceDto.shortcodes?.ecommerce_phone;
            shortcodes_ecommerce_name = ecommerceDto.shortcodes?.ecommerce_name?.Length > 50 ? ecommerceDto.shortcodes?.ecommerce_name?.Substring(0, 50) : ecommerceDto.shortcodes?.ecommerce_name;
            shortcodes_app_name = ecommerceDto.shortcodes?.app_name?.Length > 50 ? ecommerceDto.shortcodes?.app_name?.Substring(0, 50) : ecommerceDto.shortcodes?.app_name;

            stylesheet_text_primary = ecommerceDto.stylesheet?.text?.primary?.Length > 50 ? ecommerceDto.stylesheet?.text?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.text?.primary;
            stylesheet_table_primary = ecommerceDto.stylesheet?.table?.primary?.Length > 50 ? ecommerceDto.stylesheet?.table?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.table?.primary;
            stylesheet_background_primary = ecommerceDto.stylesheet?.background?.primary?.Length > 50 ? ecommerceDto.stylesheet?.background?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.background?.primary;
            stylesheet_button_primary = ecommerceDto.stylesheet?.button?.primary?.Length > 50 ? ecommerceDto.stylesheet?.button?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.button?.primary;
            stylesheet_button_text = ecommerceDto.stylesheet?.button?.text?.Length > 50 ? ecommerceDto.stylesheet?.button?.text?.Substring(0, 50) : ecommerceDto.stylesheet?.button?.text;
            stylesheet_button_borderradius = ecommerceDto.stylesheet?.button?.borderradius?.Length > 50 ? ecommerceDto.stylesheet?.button?.borderradius?.Substring(0, 50) : ecommerceDto.stylesheet?.button?.borderradius;
            stylesheet_input_text = ecommerceDto.stylesheet?.input?.text?.Length > 50 ? ecommerceDto.stylesheet?.input?.text?.Substring(0, 50) : ecommerceDto.stylesheet?.input?.text;
            stylesheet_input_background = ecommerceDto.stylesheet?.input?.background?.Length > 50 ? ecommerceDto.stylesheet?.input?.background?.Substring(0, 50) : ecommerceDto.stylesheet?.input?.background;
            stylesheet_input_border = ecommerceDto.stylesheet?.input?.border?.Length > 50 ? ecommerceDto.stylesheet?.input?.border?.Substring(0, 50) : ecommerceDto.stylesheet?.input?.border;
            stylesheet_input_borderradius = ecommerceDto.stylesheet?.input?.borderradius?.Length > 50 ? ecommerceDto.stylesheet?.input?.borderradius?.Substring(0, 50) : ecommerceDto.stylesheet?.input?.borderradius;
            stylesheet_paragraph_primary = ecommerceDto.stylesheet?.paragraph?.primary?.Length > 50 ? ecommerceDto.stylesheet?.paragraph?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.paragraph?.primary;
            stylesheet_label_primary = ecommerceDto.stylesheet?.label?.primary?.Length > 50 ? ecommerceDto.stylesheet?.label?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.label?.primary;
            stylesheet_whitebox_primary = ecommerceDto.stylesheet?.whitebox?.primary?.Length > 50 ? ecommerceDto.stylesheet?.whitebox?.primary?.Substring(0, 50) : ecommerceDto.stylesheet?.whitebox?.primary;
            stylesheet_whitebox_boxshadow = ecommerceDto.stylesheet?.whitebox?.boxshadow?.Length > 50 ? ecommerceDto.stylesheet?.whitebox?.boxshadow?.Substring(0, 50) : ecommerceDto.stylesheet?.whitebox?.boxshadow;

            this.response = response;

            is_active = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.is_active);
            maintenance_mode_global = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.maintenance_mode_global);
            is_test = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.is_test);
            deny_search_index = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.deny_search_index);
            collect_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.collect_enabled);
            has_ecommerce_integration = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.has_ecommerce_integration);
            has_info_by_customer_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.has_info_by_customer_enabled);
            hotjar_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.hotjar_enabled);
            retention_free_shipping = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.retention_free_shipping);
            show_quick_selection = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.show_quick_selection);
            maintenance_mode = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.maintenance_mode);
            enable_price_visibility = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.enable_price_visibility);
            shipping_address_change_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.shipping_address_change_enabled);
            request_evaluation_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.request_evaluation_enabled);
            enable_transportation_priority_by_preferences = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.enable_transportation_priority_by_preferences);
            force_courier_show_when_postage_block_by_pickup_in_use = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.force_courier_show_when_postage_block_by_pickup_in_use);
            refund_pix_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.refund_pix_enabled);
            allow_refund_for_third_parties_by_pix = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.allow_refund_for_third_parties_by_pix);
            could_be_retained = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.could_be_retained);
            cashback_allow_third_party = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.cashback_allow_third_party);
            request_pix_key_by_customer_interface = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.request_pix_key_by_customer_interface);
            correios_checklist_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.correios_checklist_enabled);
            request_customer_invoice_xml_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.request_customer_invoice_xml_enabled);
            split_reverses_by_seller = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.split_reverses_by_seller);
            group_products_with_unregistered_sellers = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.group_products_with_unregistered_sellers);
            care_plus_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.care_plus_enabled);
            hide_courier_enabled = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.hide_courier_enabled);
            show_product_images = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.show_product_images);

            reverse_actions_customization_online_exchange = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.reverse_actions_customization?.online?.exchange);
            reverse_actions_customization_online_return = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.reverse_actions_customization?.online?.@return);
            reverse_actions_customization_offline_exchange = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.reverse_actions_customization?.offline?.exchange);
            reverse_actions_customization_offline_return = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceDto.reverse_actions_customization?.offline?.@return);

            last_order_report_date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceDto.last_order_report_date);
            address_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceDto.address?.updated_at);
            address_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceDto.address?.deleted_at);

            messages_pages = ecommerceDto.messages?.pages != null ? new MessagePage(ecommerceDto.messages.pages, 1, id) : new MessagePage();

            messages_mails = ecommerceDto.messages?.mails != null ? new MessageMail(ecommerceDto.messages.mails, 1, id) : new MessageMail();

            ecommerceDto.reasons?.ForEach(x =>
            {
                reasons.Add(new Reason(x));
            });

            var stringValues = GenerateAlphabeticalOrderedStringValuesList(ecommerceDto);

            for (int i = 0; i < stringValues.Count; i++)
            {
                ecommerce_string_values.Add(new StringListValue(i + 1, id, stringValues[i]));
            }
        }

        private List<string> GenerateAlphabeticalOrderedStringValuesList(Dtos.Reads.Me.Ecommerce ecommerceDto)
        {
            List<string> strings = [];

            ecommerceDto.enabled_reverse_types?.ForEach(x => { strings.Add(x); });

            ecommerceDto.refund_exchange_options?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.app?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.beta?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.care?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.loyalty?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.money?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.omnichannel?.ForEach(x => { strings.Add(x); });

            ecommerceDto.modules?.retention?.ForEach(x => { strings.Add(x); });

            return strings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
