using Domain.AfterSale.Dtos.Me;
using FluentValidation;

namespace Application.AfterSale.Validations.Me
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
