using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClientesEnderecosEntregaValidator : AbstractValidator<B2CConsultaClientesEnderecosEntrega>
    {
        public B2CConsultaClientesEnderecosEntregaValidator()
        {
            RuleFor(x => x.id_endereco_entrega)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_endereco_entrega));

            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_erp));

            RuleFor(x => x.cod_cliente_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_b2c));

            RuleFor(x => x.endereco_cliente)
                .MaximumLength(250)
                .WithMessage("O campo 'endereco_cliente' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.endereco_cliente));

            RuleFor(x => x.numero_rua_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'numero_rua_cliente' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_rua_cliente));

            RuleFor(x => x.complemento_end_cli)
                .MaximumLength(60)
                .WithMessage("O campo 'complemento_end_cli' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complemento_end_cli));

            RuleFor(x => x.cep_cliente)
                .MaximumLength(9)
                .WithMessage("O campo 'cep_cliente' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep_cliente));

            RuleFor(x => x.bairro_cliente)
                .MaximumLength(60)
                .WithMessage("O campo 'bairro_cliente' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bairro_cliente));

            RuleFor(x => x.cidade_cliente)
                .MaximumLength(40)
                .WithMessage("O campo 'cidade_cliente' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cidade_cliente));

            RuleFor(x => x.uf_cliente)
                .MaximumLength(2)
                .WithMessage("O campo 'uf_cliente' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.uf_cliente));

            RuleFor(x => x.descricao)
                .MaximumLength(250)
                .WithMessage("O campo 'descricao' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.principal)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.principal));

            RuleFor(x => x.id_cidade)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cidade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
