using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// apontamento novo (sem id): atividade, data sem horas, tipo de horário e autor são obrigatórios
    /// </summary>
    public class TimeAppointmentValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.TimeAppointment>
    {
        public TimeAppointmentValidator()
        {
            When(x => !(x.id > 0), () =>
            {
                RuleFor(x => x.activity)
                    .NotEmpty()
                    .WithMessage("O campo 'activity' do apontamento é obrigatório.");

                RuleFor(x => x.date)
                    .NotNull()
                    .WithMessage("O campo 'date' do apontamento é obrigatório.");

                RuleFor(x => x.workTypeName)
                    .NotEmpty()
                    .WithMessage("O campo 'workTypeName' do apontamento é obrigatório.");

                RuleFor(x => x.createdBy)
                    .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                    .WithMessage("O campo 'createdBy.id' do apontamento é obrigatório.");
            });

            RuleFor(x => x.date)
                .Must(x => x!.Value.TimeOfDay == TimeSpan.Zero)
                .WithMessage("O campo 'date' do apontamento deve ter as horas zeradas (ex.: 2026-10-22T00:00:00).")
                .When(x => x.date is not null);
        }
    }
}
