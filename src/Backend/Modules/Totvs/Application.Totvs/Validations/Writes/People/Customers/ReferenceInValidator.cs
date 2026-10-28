using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// referência (ReferenceInDto): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class ReferenceInValidator : AbstractValidator<ReferenceIn>
    {
        public ReferenceInValidator()
        {
            RuleFor(x => x.sequence)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'sequence' deve ter até 3 dígitos (0 zera).")
                .When(x => x.sequence is not null);

            RuleFor(x => x.type)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Reference))
                .WithMessage($"O campo 'type' só aceita {CustomerRules.Names(CustomerEnumNames.Reference)}.");

            RuleFor(x => x.description)
                .MaximumLength(20)
                .WithMessage("O campo 'description' aceita até 20 caracteres.");

            RuleFor(x => x.phoneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'phoneNumber' aceita até 20 caracteres.");

            RuleFor(x => x.responsiblePersonName)
                .MaximumLength(60)
                .WithMessage("O campo 'responsiblePersonName' aceita até 60 caracteres.");
        }
    }
}
