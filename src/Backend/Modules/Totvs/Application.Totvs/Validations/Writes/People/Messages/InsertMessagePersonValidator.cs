using Domain.Totvs.Dtos.Writes.People.Messages;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Messages
{
    /// <summary>
    /// valida o corpo da inclusão de mensagens no controller, antes de chamar a Totvs (corpo inválido volta 400). Tamanhos do swagger da API
    /// Person (InsertMessagePersonCommand) e a regra "um ou outro" da pessoa
    /// </summary>
    public class InsertMessagePersonValidator : AbstractValidator<InsertMessagePerson>
    {
        public InsertMessagePersonValidator()
        {
            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.personCpfCnpj) != (x.personCode is null))
                .WithName("personCode")
                .WithMessage("Informe exatamente um entre 'personCode' e 'personCpfCnpj'.");

            RuleFor(x => x.personCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'personCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.personCode is not null);

            RuleFor(x => x.personCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'personCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.personCpfCnpj));

            RuleFor(x => x.messages)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma mensagem em 'messages'.");

            RuleForEach(x => x.messages)
                .ChildRules(message =>
                {
                    message.RuleFor(m => m.description)
                        .NotEmpty()
                        .WithMessage("O campo 'description' é obrigatório.")
                        .MaximumLength(200)
                        .WithMessage("O campo 'description' aceita até 200 caracteres.");

                    // inteiro não anulável no swagger: omitido, a Totvs receberia 0, que não é uma prioridade do PESFL151
                    message.RuleFor(m => m.priority)
                        .NotNull()
                        .WithMessage("O campo 'priority' é obrigatório (código cadastrado no PESFL151).")
                        .GreaterThan(0)
                        .WithMessage("O campo 'priority' deve ser maior que zero.");
                })
                .When(x => x.messages is not null);
        }
    }
}
