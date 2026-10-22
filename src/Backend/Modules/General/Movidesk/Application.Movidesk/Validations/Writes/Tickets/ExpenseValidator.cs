using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// despesa nova (sem id): tipo, autor, data (não futura) e quantidade ou valor são obrigatórios
    /// </summary>
    public class ExpenseValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.Expense>
    {
        public ExpenseValidator()
        {
            When(x => !(x.id > 0), () =>
            {
                RuleFor(x => x.type)
                    .NotEmpty()
                    .WithMessage("O campo 'type' da despesa é obrigatório.");

                RuleFor(x => x.createdBy)
                    .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                    .WithMessage("O campo 'createdBy.id' da despesa é obrigatório.");

                RuleFor(x => x.date)
                    .NotNull()
                    .WithMessage("O campo 'date' da despesa é obrigatório.");

                RuleFor(x => x)
                    .Must(x => x.quantity is not null || x.value is not null)
                    .WithMessage("A despesa precisa de 'quantity' ou 'value'.");
            });

            RuleFor(x => x.date)
                .Must(x => x <= DateTime.UtcNow)
                .WithMessage("O campo 'date' da despesa não pode ser uma data futura.")
                .When(x => x.date is not null);
        }
    }
}
