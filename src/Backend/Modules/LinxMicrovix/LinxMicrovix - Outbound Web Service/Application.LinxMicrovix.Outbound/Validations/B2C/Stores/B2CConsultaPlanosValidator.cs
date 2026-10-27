using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaPlanosValidator : AbstractValidator<B2CConsultaPlanos>
    {
        public B2CConsultaPlanosValidator()
        {
            RuleFor(x => x.plano)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'plano' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_plano)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_plano' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_plano));

            RuleFor(x => x.forma_pagamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.forma_pagamento));

            RuleFor(x => x.qtde_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_parcelas));

            RuleFor(x => x.valor_minimo_parcela)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_minimo_parcela));

            RuleFor(x => x.indice)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.indice));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.desativado)
                .MaximumLength(1)
                .WithMessage("O campo 'desativado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desativado));

            RuleFor(x => x.tipo_plano)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_plano' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_plano));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.usa_tef)
                .MaximumLength(1)
                .WithMessage("O campo 'usa_tef' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.usa_tef));
        }
    }
}
