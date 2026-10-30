using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisDadosValidator : AbstractValidator<B2CConsultaB2CProdutosCamposAdicionaisDados>
    {
        public B2CConsultaB2CProdutosCamposAdicionaisDadosValidator()
        {
            RuleFor(x => x.id_b2c_produtos_campos_adicionais_dados)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais_dados' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_b2c_produtos_campos_adicionais_opcoes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_produtos_campos_adicionais_opcoes));

            RuleFor(x => x.id_b2c_produtos_campos_adicionais)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_produtos_campos_adicionais));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
