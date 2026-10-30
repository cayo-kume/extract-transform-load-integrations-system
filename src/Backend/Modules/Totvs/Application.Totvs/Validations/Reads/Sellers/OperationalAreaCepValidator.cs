using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Sellers
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Seller não dá tamanho dos textos (colunas do
    /// Script008_Create_Totvs_Sellers_Tables)
    /// </summary>
    public class OperationalAreaCepValidator : AbstractValidator<OperationalAreaCep>
    {
        public OperationalAreaCepValidator()
        {
            RuleFor(x => x.operationalAreaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'operationalAreaCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cep)
                .NotEmpty()
                .WithMessage("O campo 'cep' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'cep' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.cep)
                .Matches(@"^\d{5}-?\d{3}$")
                .WithMessage("O campo 'cep' deve ter 8 dígitos.")
                .When(x => !string.IsNullOrEmpty(x.cep));
        }
    }
}
