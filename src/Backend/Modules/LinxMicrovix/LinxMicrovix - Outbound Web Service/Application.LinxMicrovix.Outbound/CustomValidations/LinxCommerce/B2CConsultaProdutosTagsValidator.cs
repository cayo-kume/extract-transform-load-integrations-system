using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosTagsValidator : AbstractValidator<B2CConsultaProdutosTags>
    {
        public B2CConsultaProdutosTagsValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_tags_produtos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_tags_produtos));

            RuleFor(x => x.id_b2c_tags)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_tags));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.descricao_b2c_tags)
                .MaximumLength(300)
                .WithMessage("O campo 'descricao_b2c_tags' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_b2c_tags));
        }
    }
}
