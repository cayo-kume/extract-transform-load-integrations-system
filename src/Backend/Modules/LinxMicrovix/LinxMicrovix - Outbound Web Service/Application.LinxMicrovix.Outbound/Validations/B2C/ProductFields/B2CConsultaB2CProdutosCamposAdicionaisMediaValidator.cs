using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisMediaValidator : AbstractValidator<B2CConsultaB2CProdutosCamposAdicionaisMedia>
    {
        public B2CConsultaB2CProdutosCamposAdicionaisMediaValidator()
        {
            RuleFor(x => x.id_b2c_produtos_campos_adicionais_media)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais_media' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.url)
                .MaximumLength(400)
                .WithMessage("O campo 'url' deve ter no máximo 400 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url));

            RuleFor(x => x.tipo_media_descricao)
                .MaximumLength(50)
                .WithMessage("O campo 'tipo_media_descricao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_media_descricao));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.media_principal)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.media_principal));

            RuleFor(x => x.tipo_media_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_media_id));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
