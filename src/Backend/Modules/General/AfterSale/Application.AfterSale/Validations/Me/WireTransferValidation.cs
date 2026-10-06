using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class WireTransferValidation : AbstractValidator<WireTransfer>
    {
        public WireTransferValidation()
        {
            RuleFor(x => x.ask_transfer_bank_account_info)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}
