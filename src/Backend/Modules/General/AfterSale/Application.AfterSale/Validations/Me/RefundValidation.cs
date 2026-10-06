using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class RefundValidation : AbstractValidator<Refund>
    {
        public RefundValidation()
        {
            RuleFor(x => x.approved)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_refund)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.refund_nps)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_wire_transfer)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_product)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_voucher)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_credit)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.ask_wire_transfer_data)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.paid_hard_retention)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.ask_wire_transfer_data_update)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.review)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}
