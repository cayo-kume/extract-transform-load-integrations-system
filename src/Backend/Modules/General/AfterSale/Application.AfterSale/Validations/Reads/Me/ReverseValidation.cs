using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class ReverseValidation : AbstractValidator<Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.invalid_courier_contract)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.awaiting_with_freight_by_customer)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.courier_collect_scheduling)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.courier_collect_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.estafeta_post_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.manual_modal_change_correio_code_generated_collect)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.manual_modal_change_correio_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.correio_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.send4_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.pending)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.generic_courier_collect_requested_date_changed_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.locker_post_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.correio_code_expiring)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.correio_collect_change_to_post)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.correio_tracking_posted)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.correio_tracking_delivered)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.ecommerce_store_reverse)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.products_received)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_refused)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_canceled)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_reopened)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_returned_invoice_solicitation)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_returned_invoice_solicitation_partially_missing)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.awaiting_without_process)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse_with_manual_processing)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.generic_courier_collect_info)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.return_point_code_generated)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.return_point_code_expiring)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.awaiting_collect_scheduling)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.awaiting_collect_scheduling_reminder)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}
