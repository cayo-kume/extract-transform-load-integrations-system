using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// rede social (SocialNetworkInDto): sequence só na alteração; sem ela, inclui: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class SocialNetworkInValidator : AbstractValidator<SocialNetworkIn>
    {
        public SocialNetworkInValidator()
        {
            RuleFor(x => x.sequence)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'sequence' deve ter até 3 dígitos (0 zera).")
                .When(x => x.sequence is not null);

            RuleFor(x => x.typeCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'typeCode' deve ter até 2 dígitos (0 zera).")
                .When(x => x.typeCode is not null);

            RuleFor(x => x.address)
                .MaximumLength(20)
                .WithMessage("O campo 'address' aceita até 20 caracteres.");
        }
    }
}
