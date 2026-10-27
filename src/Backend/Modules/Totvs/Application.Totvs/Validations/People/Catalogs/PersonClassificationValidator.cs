using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.People.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script033_Create_Totvs_Person_Catalogs_Tables
    /// </summary>
    public class PersonClassificationValidator : AbstractValidator<PersonClassification>
    {
        public PersonClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.typeDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'typeDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeDescription));

            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.description)
                .MaximumLength(60)
                .WithMessage("O campo 'description' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
