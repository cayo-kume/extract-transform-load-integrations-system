using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// ação enviada no POST ou no PATCH. Sem id a ação é nova e precisa de tipo e descrição; com id só os campos enviados mudam
    /// </summary>
    public class ActionValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.Action>
    {
        public ActionValidator()
        {
            RuleFor(x => x.type)
                .Must(x => x is 1 or 2)
                .WithMessage("O campo 'type' da ação deve ser 1 (interna) ou 2 (pública).")
                .When(x => !(x.id > 0) || x.type is not null);

            RuleFor(x => x.description)
                .NotEmpty()
                .WithMessage("O campo 'description' da ação é obrigatório.")
                .When(x => !(x.id > 0));

            RuleFor(x => x.createdBy)
                .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                .WithMessage("O campo 'createdBy.id' da ação é obrigatório quando há apontamentos.")
                .When(x => x.timeAppointments?.Count > 0);

            RuleForEach(x => x.timeAppointments)
                .SetValidator(new TimeAppointmentValidator());

            RuleForEach(x => x.expenses)
                .SetValidator(new ExpenseValidator());
        }
    }
}
