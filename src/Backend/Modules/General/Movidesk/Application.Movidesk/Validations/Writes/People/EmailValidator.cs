using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// e-mail: tipo, endereço válido e indicador de principal são obrigatórios
    /// </summary>
    public class EmailValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Email>
    {
        public EmailValidator()
        {
            RuleFor(x => x.emailType)
                .NotEmpty()
                .WithMessage("O campo 'emailType' do e-mail é obrigatório.");

            RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage("O campo 'email' é obrigatório.")
                .EmailAddress()
                .WithMessage("O campo 'email' deve ser um e-mail válido.");

            RuleFor(x => x.isDefault)
                .NotNull()
                .WithMessage("O campo 'isDefault' do e-mail é obrigatório.");
        }
    }
}
