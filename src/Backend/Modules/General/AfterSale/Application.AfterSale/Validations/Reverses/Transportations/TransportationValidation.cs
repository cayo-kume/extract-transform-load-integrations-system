using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.Transportations
{
    public class TransportationValidation : AbstractValidator<string>
    {
        public TransportationValidation()
        {
            RuleFor(x => x)
                .MaximumLength(50)
                .WithMessage("O campo 'transportations' deve ter no máximo 50 caracteres.");
        }
    }
}
