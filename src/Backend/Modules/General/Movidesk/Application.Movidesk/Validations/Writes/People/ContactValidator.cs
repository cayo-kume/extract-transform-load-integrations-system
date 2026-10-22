using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// contato: tipo, descrição e indicador de principal são obrigatórios
    /// </summary>
    public class ContactValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.contactType)
                .NotEmpty()
                .WithMessage("O campo 'contactType' do contato é obrigatório.");

            RuleFor(x => x.contact)
                .NotEmpty()
                .WithMessage("O campo 'contact' do contato é obrigatório.");

            RuleFor(x => x.isDefault)
                .NotNull()
                .WithMessage("O campo 'isDefault' do contato é obrigatório.");
        }
    }
}
