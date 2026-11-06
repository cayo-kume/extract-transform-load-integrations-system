using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Refunds.Actions
{
    public class ActionValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Action>
    {
        public ActionValidation()
        {
            // o id é o código da ação ("refund", ...), texto
            RuleFor(x => x.id)
                .NotEmpty()
                .WithMessage("O campo 'id' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'id' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.name)
                .MaximumLength(50)
                .WithMessage("O campo 'name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));
        }
    }
}
