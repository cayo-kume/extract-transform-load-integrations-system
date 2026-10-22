using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// criação de pessoa (POST): barra a chamada antes da API, porque 3 erros seguidos bloqueiam a API por 60 s
    /// </summary>
    public class PostPersonValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Person>
    {
        public PostPersonValidator()
        {
            RuleFor(x => x.isActive)
                .NotNull()
                .WithMessage("O campo 'isActive' é obrigatório.");

            RuleFor(x => x.personType)
                .Must(x => x is 1 or 2 or 4)
                .WithMessage("O campo 'personType' é obrigatório: 1 (pessoa), 2 (empresa) ou 4 (departamento).");

            RuleFor(x => x.profileType)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'profileType' é obrigatório: 1 (agente), 2 (cliente) ou 3 (agente e cliente).");

            RuleFor(x => x.businessName)
                .NotEmpty()
                .WithMessage("O campo 'businessName' é obrigatório.");

            // obrigatório e no mesmo formato da rota People/{docCompany}/{id}, para a pessoa criada poder ser lida, alterada e
            // excluída por ela. Sem id, a Movidesk gera um Cód. Ref. com hífen (ex.: 9a3b9d8a-376e-4317-, teste de 22/10/2026),
            // que a rota não aceita
            RuleFor(x => x.id)
                .NotEmpty()
                .WithMessage("O campo 'id' (Cód. Ref.) é obrigatório.")
                .Matches(@"^[A-Za-z0-9]{1,64}$")
                .WithMessage("O campo 'id' (Cód. Ref.) deve ter só letras e números, até 64 caracteres.");

            // agente: perfil de acesso e ao menos uma equipe
            When(x => x.profileType is 1 or 3, () =>
            {
                RuleFor(x => x.accessProfile)
                    .NotEmpty()
                    .WithMessage("Agente precisa do 'accessProfile' (perfil de acesso cadastrado na Movidesk).");

                RuleFor(x => x.teams)
                    .NotEmpty()
                    .WithMessage("Agente precisa de ao menos uma equipe em 'teams'.");
            });

            Include(new PersonRulesValidator());
        }
    }
}
