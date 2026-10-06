using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Me
{
    public class Ecommerce
    {
        public string id { get; init; }
        public string uuid { get; init; }
        public string company_name { get; init; }
        public string trade_name { get; init; }
        public string display_name { get; init; }
        public string url { get; init; }
        public string email { get; init; }
        public string document { get; init; }
        public string address_id { get; init; }
        public string last_order_report_date { get; init; }
        public string segment { get; init; }
        public string app_name { get; init; }
        public string logo { get; init; }
        public string terms { get; init; }
        public string limit_value_collect { get; init; }
        public string field_order_id_label { get; init; }
        public string field_order_id_placeholder { get; init; }
        public string field_confirmation_label_email { get; init; }
        public string field_confirmation_placeholder_email { get; init; }
        public string field_confirmation_label_document { get; init; }
        public string field_confirmation_placeholder_document { get; init; }
        public string field_confirmation_type { get; init; }
        public string retention_method { get; init; }
        public string retention_layout_version { get; init; }
        public string request_evaluation_method { get; init; }
        public string theme { get; init; }

        public string is_active { get; init; }
        public string maintenance_mode_global { get; init; }
        public string is_test { get; init; }
        public string deny_search_index { get; init; }
        public string collect_enabled { get; init; }
        public string has_ecommerce_integration { get; init; }
        public string has_info_by_customer_enabled { get; init; }
        public string hotjar_enabled { get; init; }
        public string retention_free_shipping { get; init; }
        public string show_quick_selection { get; init; }
        public string maintenance_mode { get; init; }
        public string enable_price_visibility { get; init; }
        public string shipping_address_change_enabled { get; init; }
        public string request_evaluation_enabled { get; init; }
        public string enable_transportation_priority_by_preferences { get; init; }
        public string force_courier_show_when_postage_block_by_pickup_in_use { get; init; }
        public string refund_pix_enabled { get; init; }
        public string allow_refund_for_third_parties_by_pix { get; init; }
        public string could_be_retained { get; init; }
        public string cashback_allow_third_party { get; init; }
        public string request_pix_key_by_customer_interface { get; init; }
        public string correios_checklist_enabled { get; init; }
        public string request_customer_invoice_xml_enabled { get; init; }
        public string split_reverses_by_seller { get; init; }
        public string group_products_with_unregistered_sellers { get; init; }
        public string care_plus_enabled { get; init; }
        public string hide_courier_enabled { get; init; }
        public string show_product_images { get; init; }

        public string ecommerce_group_id { get; init; }
        public string hubspot_code { get; init; }
        public string phone { get; init; }
        public string oauth_client_id { get; init; }
        public string provider_id { get; init; }
        public string registration_origin { get; init; }
        public string field_product_comment_label { get; init; }
        public string brand_id { get; init; }
        public string field_product_comment_placeholder { get; init; }
        public string hotjar_id { get; init; }
        public string bank_agency_max_length { get; init; }
        public string correios_checklist_default_type { get; init; }

        public List<string> refund_exchange_options { get; init; }
        public List<string> enabled_reverse_types { get; init; }

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

        public List<Reason> reasons { get; init; }

        public Address address { get; init; }
        public Localization localization { get; init; }
        public Message messages { get; init; }
        public Shortcode shortcodes { get; init; }
        public Stylesheet stylesheet { get; init; }
        public ReverseActionsCustomization reverse_actions_customization { get; init; }
        public ReverseActionCustomizationLabel reverse_actions_customization_labels { get; init; }
        public Module modules { get; init; }
    }
}
