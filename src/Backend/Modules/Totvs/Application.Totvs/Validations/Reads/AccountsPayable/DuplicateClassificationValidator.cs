using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.AccountsPayable
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Accounts Payable (colunas do Script020_Create_Totvs_Accounts_Payable_Tables)
    /// </summary>
    public class DuplicateClassificationValidator : AbstractValidator<DuplicateClassification>
    {
        public DuplicateClassificationValidator()
        {
            RuleFor(x => x.classificationType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'classificationType' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.classificationCode)
                .MaximumLength(10)
                .WithMessage("O campo 'classificationCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificationCode));

            RuleFor(x => x.classificationTypeDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'classificationTypeDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificationTypeDescription));

            RuleFor(x => x.classificationDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'classificationDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificationDescription));
        }
    }
}
