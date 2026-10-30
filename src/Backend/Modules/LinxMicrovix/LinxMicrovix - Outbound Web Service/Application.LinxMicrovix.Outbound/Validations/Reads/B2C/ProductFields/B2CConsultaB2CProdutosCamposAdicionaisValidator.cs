using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisValidator : AbstractValidator<B2CConsultaB2CProdutosCamposAdicionais>
    {
        public B2CConsultaB2CProdutosCamposAdicionaisValidator()
        {
            RuleFor(x => x.id_b2c_produtos_campos_adicionais)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_produtos_campos_adicionais' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.decimais)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.decimais));

            RuleFor(x => x.multipla_escolha)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.multipla_escolha));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.nivel_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.nivel_id));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
