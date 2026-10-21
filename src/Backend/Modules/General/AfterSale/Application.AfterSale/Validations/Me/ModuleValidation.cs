using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class ModuleValidation : AbstractValidator<Module>
    {
        public ModuleValidation()
        {
            RuleForEach(x => x.whitelabel)
                .MaximumLength(300)
                .WithMessage("O campo 'whitelabel' deve ter no máximo 300 caracteres.")
                .When(x => x.whitelabel is { Count: > 0 }); ;

            RuleForEach(x => x.security)
                .MaximumLength(300)
                .WithMessage("O campo 'security' deve ter no máximo 300 caracteres.")
                .When(x => x.security  is { Count: > 0 });;

            RuleForEach(x => x.order)
                .MaximumLength(300)
                .WithMessage("O campo 'order' deve ter no máximo 300 caracteres.")
                .When(x => x.order  is { Count: > 0 });;

            RuleForEach(x => x.logistic)
                .MaximumLength(300)
                .WithMessage("O campo 'logistic' deve ter no máximo 300 caracteres.")
                .When(x => x.logistic  is { Count: > 0 });;

            RuleForEach(x => x.insurance)
                .MaximumLength(300)
                .WithMessage("O campo 'insurance' deve ter no máximo 300 caracteres.")
                .When(x => x.insurance  is { Count: > 0 });;

            RuleForEach(x => x.fiscal)
                .MaximumLength(300)
                .WithMessage("O campo 'fiscal' deve ter no máximo 300 caracteres.")
                .When(x => x.fiscal  is { Count: > 0 });;

            RuleForEach(x => x.retention)
                .MaximumLength(300)
                .WithMessage("O campo 'retention' deve ter no máximo 300 caracteres.")
                .When(x => x.retention  is { Count: > 0 });;

            RuleForEach(x => x.omnichannel)
                .MaximumLength(300)
                .WithMessage("O campo 'omnichannel' deve ter no máximo 300 caracteres.")
                .When(x => x.omnichannel  is { Count: > 0 });;

            RuleForEach(x => x.money)
                .MaximumLength(300)
                .WithMessage("O campo 'money' deve ter no máximo 300 caracteres.")
                .When(x => x.money  is { Count: > 0 });;

            RuleForEach(x => x.loyalty)
                .MaximumLength(300)
                .WithMessage("O campo 'loyalty' deve ter no máximo 300 caracteres.")
                .When(x => x.loyalty  is { Count: > 0 });;

            RuleForEach(x => x.care)
                .MaximumLength(300)
                .WithMessage("O campo 'care' deve ter no máximo 300 caracteres.")
                .When(x => x.care  is { Count: > 0 });;

            RuleForEach(x => x.beta)
                .MaximumLength(300)
                .WithMessage("O campo 'beta' deve ter no máximo 300 caracteres.")
                .When(x => x.beta  is { Count: > 0 });;

            RuleForEach(x => x.app)
                .MaximumLength(300)
                .WithMessage("O campo 'app' deve ter no máximo 300 caracteres.")
                .When(x => x.app is { Count: > 0 });;
        }
    }
}
