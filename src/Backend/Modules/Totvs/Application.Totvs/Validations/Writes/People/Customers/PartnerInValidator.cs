using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// sócio da pessoa jurídica (PartnerInDto): pelo CPF/CNPJ ou pelo código da pessoa: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class PartnerInValidator : AbstractValidator<PartnerIn>
    {
        public PartnerInValidator()
        {
            RuleFor(x => x.partnerCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'partnerCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.partnerCode is not null);

            RuleFor(x => x.percentageParticipation)
                .InclusiveBetween(0, 999.99m)
                .WithMessage("O campo 'percentageParticipation' aceita até 3 inteiros (0 zera).")
                .PrecisionScale(5, 2, true)
                .WithMessage("O campo 'percentageParticipation' aceita até 2 casas decimais.")
                .When(x => x.percentageParticipation is not null);

            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.cpfPartner) != (x.partnerCode is null))
                .WithName("cpfPartner")
                .WithMessage("Informe exatamente um entre 'cpfPartner' e 'partnerCode' no sócio.");

            RuleFor(x => x.cpfPartner)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'cpfPartner' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.cpfPartner));

            RuleFor(x => x.percentageParticipation)
                .LessThanOrEqualTo(100)
                .WithMessage("O campo 'percentageParticipation' vai até 100.")
                .When(x => x.percentageParticipation is not null);
        }
    }
}
