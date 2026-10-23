using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script013_Create_Totvs_People_Tables)
    /// </summary>
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(20)
                .WithMessage("O campo 'typeName' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.function)
                .MaximumLength(20)
                .WithMessage("O campo 'function' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.function));

            RuleFor(x => x.phoneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'phoneNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phoneNumber));

            RuleFor(x => x.cellNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'cellNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cellNumber));

            RuleFor(x => x.email)
                .MaximumLength(60)
                .WithMessage("O campo 'email' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.birthDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.birthDate));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));
        }
    }
}
