using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class PageValidation : AbstractValidator<Page>
    {
        public PageValidation()
        {
            RuleFor(x => x.introduction)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.select_source)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.order)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.insurance_checklist)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.insurance_loading_failure)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.shipping)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.resume)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.deadline_for_insurance_cancellation_expired)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.finish)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.finish_thank)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.customer)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.customer_products)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.email_by_customer)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.retention_voucher)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.retention_freight)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.existing_request_post)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.existing_request_collect)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.existing_request)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.request_follow_up)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.refund_nps)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.refund_nps_confirmation)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.products)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.cancellation_insurance)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.seller)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.retention)
                .SetValidator(new PageBodyContentValidation());

            RuleFor(x => x.bank_account_details)
                .SetValidator(new PageBodyContentValidation());
        }
    }
}
