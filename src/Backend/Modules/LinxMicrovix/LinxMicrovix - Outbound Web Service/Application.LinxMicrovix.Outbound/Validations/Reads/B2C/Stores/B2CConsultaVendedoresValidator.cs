using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaVendedoresValidator : AbstractValidator<B2CConsultaVendedores>
    {
        public B2CConsultaVendedoresValidator()
        {
            RuleFor(x => x.cod_vendedor)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_vendedor' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_vendedor)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_vendedor' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_vendedor));

            RuleFor(x => x.comissao_produtos)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.comissao_produtos));

            RuleFor(x => x.comissao_servicos)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.comissao_servicos));

            RuleFor(x => x.tipo)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.comissionado)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.comissionado));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
