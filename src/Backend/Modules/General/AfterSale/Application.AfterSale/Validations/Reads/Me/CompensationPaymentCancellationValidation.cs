using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class CompensationPaymentCancellationValidation : AbstractValidator<CompensationPaymentCancellation>
    {
        public CompensationPaymentCancellationValidation()
        {
            RuleFor(x => x.gift_card)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.refund)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.wire_transfer)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}
