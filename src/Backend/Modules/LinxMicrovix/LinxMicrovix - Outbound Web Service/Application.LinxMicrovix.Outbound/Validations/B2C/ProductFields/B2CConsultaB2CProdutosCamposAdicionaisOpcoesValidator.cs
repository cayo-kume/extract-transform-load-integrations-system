using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator : AbstractValidator<B2CConsultaB2CProdutosCamposAdicionaisOpcoes>
    {
        public B2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator()
        {
            RuleFor(x => x.id_b2c_produtos_campos_adicionais_opcoes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais_opcoes' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_b2c_produtos_campos_adicionais)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_produtos_campos_adicionais));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.categoria_nivel)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.categoria_nivel));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
