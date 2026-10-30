using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.GetReverse
{
    public class BrandValidation : AbstractValidator<Brand>
    {
        public BrandValidation()
        {
            RuleFor(x => x.name)
                .MaximumLength(50)
                .WithMessage("O campo 'name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
