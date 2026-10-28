using Domain.Totvs.Dtos.Writes.People.Messages;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Messages
{
    /// <summary>
    /// valida o corpo da alteração de mensagem no controller (corpo inválido volta 400): a mensagem pela pessoa e sequence, e a descrição,
    /// que a Totvs exige em toda alteração. O schema não diz o tamanho da descrição: vale o do POST (200)
    /// </summary>
    public class UpdateMessagePersonValidator : AbstractValidator<UpdateMessagePerson>
    {
        public UpdateMessagePersonValidator()
        {
            RuleFor(x => x.filter)
                .NotNull()
                .WithMessage("O campo 'filter' é obrigatório (pessoa e sequence da mensagem).");

            RuleFor(x => x.filter!)
                .Must(f => String.IsNullOrEmpty(f.personCpfCnpj) != (f.personCode is null))
                .WithName("filter.personCode")
                .WithMessage("Informe exatamente um entre 'filter.personCode' e 'filter.personCpfCnpj'.")
                .When(x => x.filter is not null);

            RuleFor(x => x.filter!.personCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'filter.personCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.filter?.personCode is not null);

            RuleFor(x => x.filter!.personCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'filter.personCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.filter?.personCpfCnpj));

            RuleFor(x => x.filter!.sequence)
                .NotNull()
                .WithMessage("O campo 'filter.sequence' é obrigatório (devolvido pela inclusão da mensagem).")
                .GreaterThan(0)
                .WithMessage("O campo 'filter.sequence' deve ser maior que zero.")
                .When(x => x.filter is not null);

            // a Totvs recusa a alteração sem descrição (EmptyField, testado em homologação em 28/10/2026), mesmo para só mudar a situação
            RuleFor(x => x.description)
                .NotEmpty()
                .WithMessage("O campo 'description' é obrigatório: a Totvs recusa a alteração sem ele, mesmo para só mudar a situação (repita a descrição atual).")
                .MaximumLength(200)
                .WithMessage("O campo 'description' aceita até 200 caracteres.");

            RuleFor(x => x.status)
                .IsInEnum()
                .WithMessage("O campo 'status' só aceita 1 (não lida), 2 (lida) ou 3 (cancelada).");

            RuleFor(x => x.priority)
                .GreaterThan(0)
                .WithMessage("O campo 'priority' deve ser maior que zero.")
                .When(x => x.priority is not null);
        }
    }
}
