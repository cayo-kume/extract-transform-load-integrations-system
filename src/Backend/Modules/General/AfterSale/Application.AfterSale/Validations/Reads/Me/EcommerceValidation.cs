using Domain.AfterSale.Dtos.Reads.Me;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class EcommerceValidation : AbstractValidator<Ecommerce>
    {
        public EcommerceValidation()
        {
            RuleFor(x => x.ecommerce_group_id)
                .MaximumLength(50)
                .WithMessage("O campo 'ecommerce_group_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_group_id));

            RuleFor(x => x.hubspot_code)
                .MaximumLength(50)
                .WithMessage("O campo 'hubspot_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.hubspot_code));

            RuleFor(x => x.company_name)
                .MaximumLength(120)
                .WithMessage("O campo 'company_name' deve ter no máximo 120 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.company_name));

            RuleFor(x => x.trade_name)
                .MaximumLength(120)
                .WithMessage("O campo 'trade_name' deve ter no máximo 120 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.trade_name));

            RuleFor(x => x.display_name)
                .MaximumLength(612)
                .WithMessage("O campo 'display_name' deve ter no máximo 612 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.display_name));

            RuleFor(x => x.url)
                .MaximumLength(300)
                .WithMessage("O campo 'url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url));

            RuleFor(x => x.phone)
                .MaximumLength(20)
                .WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phone));

            RuleFor(x => x.email)
                .MaximumLength(300)
                .WithMessage("O campo 'email' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.document)
                .MaximumLength(14)
                .WithMessage("O campo 'document' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document));
            
            RuleFor(x => x.segment)
                .MaximumLength(50)
                .WithMessage("O campo 'segment' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.segment));

            RuleFor(x => x.oauth_client_id)
                .MaximumLength(50)
                .WithMessage("O campo 'oauth_client_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.oauth_client_id));

            RuleFor(x => x.provider_id)
                .MaximumLength(50)
                .WithMessage("O campo 'provider_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.provider_id));

            RuleFor(x => x.registration_origin)
                .MaximumLength(50)
                .WithMessage("O campo 'registration_origin' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.registration_origin));

            RuleFor(x => x.brand_id)
                .MaximumLength(50)
                .WithMessage("O campo 'brand_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.brand_id));

            RuleFor(x => x.app_name)
                .MaximumLength(50)
                .WithMessage("O campo 'app_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.app_name));

            RuleFor(x => x.theme)
                .MaximumLength(50)
                .WithMessage("O campo 'theme' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.theme));

            RuleFor(x => x.logo)
                .MaximumLength(300)
                .WithMessage("O campo 'logo' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.logo));

            RuleFor(x => x.terms)
                .MaximumLength(300)
                .WithMessage("O campo 'terms' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.terms));

            RuleFor(x => x.limit_value_collect)
                .MaximumLength(50)
                .WithMessage("O campo 'limit_value_collect' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.limit_value_collect));

            RuleFor(x => x.retention_method)
                .MaximumLength(50)
                .WithMessage("O campo 'retention_method' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.retention_method));

            RuleFor(x => x.hotjar_id)
                .MaximumLength(50)
                .WithMessage("O campo 'hotjar_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.hotjar_id));

            RuleFor(x => x.retention_layout_version)
                .MaximumLength(50)
                .WithMessage("O campo 'retention_layout_version' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.retention_layout_version));

            RuleFor(x => x.request_evaluation_method)
                .MaximumLength(50)
                .WithMessage("O campo 'request_evaluation_method' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.request_evaluation_method));

            RuleFor(x => x.bank_agency_max_length)
                .MaximumLength(50)
                .WithMessage("O campo 'bank_agency_max_length' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bank_agency_max_length));

            RuleFor(x => x.correios_checklist_default_type)
                .MaximumLength(50)
                .WithMessage("O campo 'correios_checklist_default_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.correios_checklist_default_type));

            RuleFor(x => x.field_confirmation_type)
                .MaximumLength(50)
                .WithMessage("O campo 'field_confirmation_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_confirmation_type));

            RuleFor(x => x.field_confirmation_placeholder_document)
                .MaximumLength(50)
                .WithMessage("O campo 'field_confirmation_placeholder_document' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_confirmation_placeholder_document));

            RuleFor(x => x.field_confirmation_label_document)
                .MaximumLength(50)
                .WithMessage("O campo 'field_confirmation_label_document' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_confirmation_label_document));

            RuleFor(x => x.field_confirmation_placeholder_email)
                .MaximumLength(50)
                .WithMessage("O campo 'field_confirmation_placeholder_email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_confirmation_placeholder_email));

            RuleFor(x => x.field_confirmation_label_email)
                .MaximumLength(50)
                .WithMessage("O campo 'field_confirmation_label_email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_confirmation_label_email));

            RuleFor(x => x.field_product_comment_placeholder)
                .MaximumLength(50)
                .WithMessage("O campo 'field_product_comment_placeholder' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_product_comment_placeholder));

            RuleFor(x => x.field_product_comment_label)
                .MaximumLength(50)
                .WithMessage("O campo 'field_product_comment_label' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_product_comment_label));

            RuleFor(x => x.field_order_id_placeholder)
                .MaximumLength(50)
                .WithMessage("O campo 'field_order_id_placeholder' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_order_id_placeholder));

            RuleFor(x => x.field_order_id_label)
                .MaximumLength(50)
                .WithMessage("O campo 'field_order_id_label' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.field_order_id_label));

            RuleFor(x => x.shortcodes.ecommerce_phone)
                .MaximumLength(50)
                .WithMessage("O campo 'shortcodes_ecommerce_phone' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_phone));

            RuleFor(x => x.shortcodes.app_name)
                .MaximumLength(50)
                .WithMessage("O campo 'shortcodes_app_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.app_name));

            RuleFor(x => x.shortcodes.ecommerce_name)
                .MaximumLength(50)
                .WithMessage("O campo 'shortcodes_ecommerce_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_name));

            RuleFor(x => x.shortcodes.ecommerce_logo_email_url)
                .MaximumLength(300)
                .WithMessage("O campo 'shortcodes_ecommerce_logo_email_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_logo_email_url));

            RuleFor(x => x.shortcodes.ecommerce_logo_url)
                .MaximumLength(300)
                .WithMessage("O campo 'shortcodes_ecommerce_logo_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_logo_url));

            RuleFor(x => x.shortcodes.ecommerce_url)
                .MaximumLength(300)
                .WithMessage("O campo 'shortcodes_ecommerce_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_url));

            RuleFor(x => x.shortcodes.ecommerce_display_name)
                .MaximumLength(50)
                .WithMessage("O campo 'shortcodes_ecommerce_display_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shortcodes.ecommerce_display_name));

            RuleFor(x => x.stylesheet.background.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_background_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.background.primary));

            RuleFor(x => x.stylesheet.table.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_table_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.table.primary));

            RuleFor(x => x.stylesheet.text.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_text_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.text.primary));

            RuleFor(x => x.stylesheet.button.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_button_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.button.primary));

            RuleFor(x => x.stylesheet.button.text)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_button_text' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.button.text));

            RuleFor(x => x.stylesheet.button.borderradius)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_button_borderradius' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.button.borderradius));

            RuleFor(x => x.stylesheet.input.text)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_input_text' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.input.text));

            RuleFor(x => x.stylesheet.input.border)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_input_border' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.input.border));

            RuleFor(x => x.stylesheet.input.background)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_input_background' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.input.background));

            RuleFor(x => x.stylesheet.input.borderradius)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_input_borderradius' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.input.borderradius));

            RuleFor(x => x.stylesheet.paragraph.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_paragraph_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.paragraph.primary));

            RuleFor(x => x.stylesheet.label.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_label.primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.label.primary));

            RuleFor(x => x.stylesheet.whitebox.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_whitebox_primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.whitebox.primary));

            RuleFor(x => x.stylesheet.whitebox.boxshadow)
                .MaximumLength(50)
                .WithMessage("O campo 'stylesheet_whitebox_boxshadow' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stylesheet.whitebox.boxshadow));

            RuleFor(x => x.reverse_actions_customization_labels.online.exchange)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_actions_customization_labels_online_exchange' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization_labels.online.exchange));

            RuleFor(x => x.reverse_actions_customization_labels.online.@return)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_actions_customization_labels_online_return' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization_labels.online.@return));

            RuleFor(x => x.reverse_actions_customization_labels.offline.exchange)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_actions_customization_labels_offline_exchange' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization_labels.offline.exchange));

            RuleFor(x => x.reverse_actions_customization_labels.offline.@return)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_actions_customization_labels_offline_return' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization_labels.offline.@return));

            RuleFor(x => x.localization.language)
                .MaximumLength(50)
                .WithMessage("O campo 'localization_language' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.localization.language));

            RuleFor(x => x.localization.timezone)
                .MaximumLength(50)
                .WithMessage("O campo 'localization_timezone' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.localization.timezone));

            RuleFor(x => x.localization.currency)
                .MaximumLength(50)
                .WithMessage("O campo 'localization_currency' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.localization.currency));

            RuleFor(x => x.address.zip_code)
                .MaximumLength(9)
                .WithMessage("O campo 'address_zip_code' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.zip_code));

            RuleFor(x => x.address.city)
                .MaximumLength(40)
                .WithMessage("O campo 'address_city' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.city));

            RuleFor(x => x.address.neighborhood)
                .MaximumLength(50)
                .WithMessage("O campo 'address_neighborhood' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.neighborhood));

            RuleFor(x => x.address.number)
                .MaximumLength(20)
                .WithMessage("O campo 'address_number' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.number));

            RuleFor(x => x.address.complement)
                .MaximumLength(50)
                .WithMessage("O campo 'address_complement' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.complement));

            RuleFor(x => x.address.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address_address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.address));

            RuleFor(x => x.address.state)
                .MaximumLength(50)
                .WithMessage("O campo 'address_state' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.state));

            RuleFor(x => x.address.country)
                .MaximumLength(50)
                .WithMessage("O campo 'address_country' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.country));

            RuleFor(x => x.address.lat)
                .MaximumLength(50)
                .WithMessage("O campo 'address_lat' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.lat));

            RuleFor(x => x.address.@long)
                .MaximumLength(50)
                .WithMessage("O campo 'address_long' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address.@long));

            RuleFor(x => x.address.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.address.id));

            RuleFor(x => x.address.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.address.updated_at));

            RuleFor(x => x.address.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.address.deleted_at));

            RuleFor(x => x.last_order_report_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.last_order_report_date));

            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.uuid)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.uuid));

            RuleFor(x => x.is_active)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_active));

            RuleFor(x => x.maintenance_mode_global)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.maintenance_mode_global));

            RuleFor(x => x.is_test)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_test));

            RuleFor(x => x.deny_search_index)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.deny_search_index));

            RuleFor(x => x.has_ecommerce_integration)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.has_ecommerce_integration));

            RuleFor(x => x.has_info_by_customer_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.has_info_by_customer_enabled));

            RuleFor(x => x.collect_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.collect_enabled));

            RuleFor(x => x.hotjar_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hotjar_enabled));

            RuleFor(x => x.retention_free_shipping)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.retention_free_shipping));

            RuleFor(x => x.show_quick_selection)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.show_quick_selection));

            RuleFor(x => x.maintenance_mode)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.maintenance_mode));

            RuleFor(x => x.enable_price_visibility)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.enable_price_visibility));

            RuleFor(x => x.shipping_address_change_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.shipping_address_change_enabled));

            RuleFor(x => x.request_evaluation_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.request_evaluation_enabled));

            RuleFor(x => x.force_courier_show_when_postage_block_by_pickup_in_use)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.force_courier_show_when_postage_block_by_pickup_in_use));

            RuleFor(x => x.enable_transportation_priority_by_preferences)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.enable_transportation_priority_by_preferences));

            RuleFor(x => x.refund_pix_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.refund_pix_enabled));

            RuleFor(x => x.allow_refund_for_third_parties_by_pix)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.allow_refund_for_third_parties_by_pix));

            RuleFor(x => x.could_be_retained)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.could_be_retained));

            RuleFor(x => x.cashback_allow_third_party)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.cashback_allow_third_party));

            RuleFor(x => x.request_pix_key_by_customer_interface)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.request_pix_key_by_customer_interface));

            RuleFor(x => x.correios_checklist_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.correios_checklist_enabled));

            RuleFor(x => x.show_product_images)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.show_product_images));

            RuleFor(x => x.hide_courier_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hide_courier_enabled));

            RuleFor(x => x.care_plus_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.care_plus_enabled));

            RuleFor(x => x.group_products_with_unregistered_sellers)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.group_products_with_unregistered_sellers));

            RuleFor(x => x.split_reverses_by_seller)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.split_reverses_by_seller));

            RuleFor(x => x.request_customer_invoice_xml_enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.request_customer_invoice_xml_enabled));

            RuleFor(x => x.reverse_actions_customization.offline.@return)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization.offline.@return));

            RuleFor(x => x.reverse_actions_customization.offline.exchange)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization.offline.exchange));

            RuleFor(x => x.reverse_actions_customization.online.@return)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization.online.@return));

            RuleFor(x => x.reverse_actions_customization.online.exchange)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.reverse_actions_customization.online.exchange));

            RuleForEach(x => x.refund_exchange_options)
                .MaximumLength(300)
                .WithMessage("O campo 'refund_exchange_options' deve ter no máximo 300 caracteres.")
                .When(x => x.refund_exchange_options is { Count: > 0 }); ;

            RuleForEach(x => x.enabled_reverse_types)
                .MaximumLength(300)
                .WithMessage("O campo 'enabled_reverse_types' deve ter no máximo 300 caracteres.")
                .When(x => x.enabled_reverse_types  is { Count: > 0 });;

            RuleForEach(x => x.reasons)
                .SetValidator(new ReasonValidation());

            RuleFor(x => x.address)
                .SetValidator(new AddressValidation());

            RuleFor(x => x.localization)
                .SetValidator(new LocalizationValidation());

            RuleFor(x => x.messages)
                .SetValidator(new MessageValidation());

            RuleFor(x => x.shortcodes)
                .SetValidator(new ShortcodeValidation());

            RuleFor(x => x.stylesheet)
                .SetValidator(new StylesheetValidation());

            RuleFor(x => x.reverse_actions_customization)
                .SetValidator(new ReverseActionsCustomizationValidation());

            RuleFor(x => x.reverse_actions_customization_labels)
                .SetValidator(new ReverseActionCustomizationLabelValidation());

            RuleFor(x => x.modules)
                .SetValidator(new ModuleValidation());
        }
    }
}
