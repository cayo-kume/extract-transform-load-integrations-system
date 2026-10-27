using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.People.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person e das colunas do Script033_Create_Totvs_Person_Catalogs_Tables
    /// </summary>
    public class EmailTypeValidator : AbstractValidator<EmailType>
    {
        public EmailTypeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
