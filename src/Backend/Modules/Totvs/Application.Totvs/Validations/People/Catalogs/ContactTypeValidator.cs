using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.People.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script026_Create_Totvs_Person_Catalogs_Tables
    /// </summary>
    public class ContactTypeValidator : AbstractValidator<ContactType>
    {
        public ContactTypeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.typeName)
                .MaximumLength(60)
                .WithMessage("O campo 'typeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
