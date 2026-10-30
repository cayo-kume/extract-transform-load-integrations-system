using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator : AbstractValidator<B2CConsultaB2CProdutosCamposAdicionaisMediaDados>
    {
        public B2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator()
        {
            RuleFor(x => x.id_b2c_produtos_campos_adicionais_media_dados)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais_media_dados' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_b2c_produtos_campos_adicionais_media)
                .MaximumLength(50)
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais_media' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id_b2c_produtos_campos_adicionais_media));

            RuleFor(x => x.codigoproduto)
                .MaximumLength(300)
                .WithMessage("O campo 'codigoproduto' deve ter no máximo 300 caracteres.")
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
