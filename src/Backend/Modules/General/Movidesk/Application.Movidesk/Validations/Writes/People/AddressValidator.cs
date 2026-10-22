using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// endereço: tipo e indicador de principal são obrigatórios
    /// </summary>
    public class AddressValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.addressType)
                .NotEmpty()
                .WithMessage("O campo 'addressType' do endereço é obrigatório.");

            RuleFor(x => x.isDefault)
                .NotNull()
                .WithMessage("O campo 'isDefault' do endereço é obrigatório.");
        }
    }
}
