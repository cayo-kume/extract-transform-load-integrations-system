using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosTabelasValidator : AbstractValidator<B2CConsultaProdutosTabelas>
    {
        public B2CConsultaProdutosTabelasValidator()
        {
            RuleFor(x => x.id_tabela)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tabela));

            RuleFor(x => x.nome_tabela)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_tabela' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_tabela));

            RuleFor(x => x.ativa)
                .MaximumLength(1)
                .WithMessage("O campo 'ativa' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.ativa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
