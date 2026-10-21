using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.History
{
    public class HistoryValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Reverses.History.Root>
    {
        public HistoryValidation()
        {
            RuleFor(x => x.link)
                .MaximumLength(250)
                .WithMessage("O campo 'link' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.link));
        }
    }
}
