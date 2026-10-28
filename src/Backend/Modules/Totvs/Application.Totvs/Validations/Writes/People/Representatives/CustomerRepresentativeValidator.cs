using Domain.Totvs.Dtos.Writes.People.Representatives;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Representatives
{
    /// <summary>
    /// valida o corpo do vínculo cliente-representante no controller (corpo inválido volta 400): os dois "um ou outro" da descrição do swagger,
    /// listas sem repetição e com até 500 clientes (o swagger não diz o máximo)
    /// </summary>
    public class CustomerRepresentativeValidator : AbstractValidator<CustomerRepresentative>
    {
        public const int MaxCustomers = 500;

        public CustomerRepresentativeValidator()
        {
            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.representativeCpfCnpj) != (x.representativeCode is null))
                .WithName("representativeCode")
                .WithMessage("Informe exatamente um entre 'representativeCode' e 'representativeCpfCnpj'.");

            RuleFor(x => x.representativeCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'representativeCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.representativeCode is not null);

            RuleFor(x => x.representativeCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'representativeCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.representativeCpfCnpj));

            RuleFor(x => x)
                .Must(x => (x.customerCodeList is { Count: > 0 }) != (x.customerCpfCnpjList is { Count: > 0 }))
                .WithName("customerCodeList")
                .WithMessage("Informe exatamente uma lista, não vazia, entre 'customerCodeList' e 'customerCpfCnpjList'.");

            RuleFor(x => x.customerCodeList)
                .Must(l => l!.All(c => c is >= 1 and <= 999_999_999))
                .WithMessage("Os códigos de 'customerCodeList' devem ter de 1 a 9 dígitos.")
                .Must(l => l!.Distinct().Count() == l!.Count)
                .WithMessage("O mesmo cliente aparece mais de uma vez em 'customerCodeList'.")
                .Must(l => l!.Count <= MaxCustomers)
                .WithMessage($"Até {MaxCustomers} clientes por chamada.")
                .When(x => x.customerCodeList is { Count: > 0 });

            RuleFor(x => x.customerCpfCnpjList)
                .Must(l => l!.All(c => c is not null && System.Text.RegularExpressions.Regex.IsMatch(c, @"^(\d{11}|\d{14})$")))
                .WithMessage("Os documentos de 'customerCpfCnpjList' devem ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .Must(l => l!.Distinct().Count() == l!.Count)
                .WithMessage("O mesmo cliente aparece mais de uma vez em 'customerCpfCnpjList'.")
                .Must(l => l!.Count <= MaxCustomers)
                .WithMessage($"Até {MaxCustomers} clientes por chamada.")
                .When(x => x.customerCpfCnpjList is { Count: > 0 });
        }
    }
}
