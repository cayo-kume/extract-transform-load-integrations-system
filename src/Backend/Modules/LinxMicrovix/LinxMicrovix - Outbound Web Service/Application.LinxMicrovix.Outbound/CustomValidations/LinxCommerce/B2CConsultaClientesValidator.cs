using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClientesValidator : AbstractValidator<B2CConsultaClientes>
    {
        public B2CConsultaClientesValidator()
        {
            RuleFor(x => x.cod_cliente_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_b2c));

            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_erp));

            RuleFor(x => x.doc_cliente)
                .MaximumLength(14)
                .WithMessage("O campo 'doc_cliente' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.doc_cliente));

            RuleFor(x => x.nm_cliente)
                .MaximumLength(60)
                .WithMessage("O campo 'nm_cliente' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_cliente));

            RuleFor(x => x.nm_mae)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_mae' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_mae));

            RuleFor(x => x.nm_pai)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_pai' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_pai));

            RuleFor(x => x.nm_conjuge)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_conjuge' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_conjuge));

            RuleFor(x => x.dt_cadastro)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_cadastro));

            RuleFor(x => x.dt_nasc_cliente)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_nasc_cliente));

            RuleFor(x => x.end_cliente)
                .MaximumLength(250)
                .WithMessage("O campo 'end_cliente' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.end_cliente));

            RuleFor(x => x.complemento_end_cliente)
                .MaximumLength(50)
                .WithMessage("O campo 'complemento_end_cliente' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complemento_end_cliente));

            RuleFor(x => x.nr_rua_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'nr_rua_cliente' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nr_rua_cliente));

            RuleFor(x => x.bairro_cliente)
                .MaximumLength(60)
                .WithMessage("O campo 'bairro_cliente' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bairro_cliente));

            RuleFor(x => x.cep_cliente)
                .MaximumLength(9)
                .WithMessage("O campo 'cep_cliente' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep_cliente));

            RuleFor(x => x.cidade_cliente)
                .MaximumLength(40)
                .WithMessage("O campo 'cidade_cliente' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cidade_cliente));

            RuleFor(x => x.uf_cliente)
                .MaximumLength(2)
                .WithMessage("O campo 'uf_cliente' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.uf_cliente));

            RuleFor(x => x.fone_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'fone_cliente' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fone_cliente));

            RuleFor(x => x.fone_comercial)
                .MaximumLength(20)
                .WithMessage("O campo 'fone_comercial' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fone_comercial));

            RuleFor(x => x.cel_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'cel_cliente' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cel_cliente));

            RuleFor(x => x.email_cliente)
                .MaximumLength(50)
                .WithMessage("O campo 'email_cliente' deve ter no máximo 50 caracteres.")
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.email_cliente));

            RuleFor(x => x.rg_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'rg_cliente' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rg_cliente));

            RuleFor(x => x.rg_orgao_emissor)
                .MaximumLength(7)
                .WithMessage("O campo 'rg_orgao_emissor' deve ter no máximo 7 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rg_orgao_emissor));

            RuleFor(x => x.estado_civil_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.estado_civil_cliente));

            RuleFor(x => x.empresa_cliente)
                .MaximumLength(30)
                .WithMessage("O campo 'empresa_cliente' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.empresa_cliente));

            RuleFor(x => x.cargo_cliente)
                .MaximumLength(30)
                .WithMessage("O campo 'cargo_cliente' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cargo_cliente));

            RuleFor(x => x.sexo_cliente)
                .MaximumLength(1)
                .WithMessage("O campo 'sexo_cliente' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.sexo_cliente));

            RuleFor(x => x.dt_update)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_update));

            RuleFor(x => x.ativo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.receber_email)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.receber_email));

            RuleFor(x => x.dt_expedicao_rg)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_expedicao_rg));

            RuleFor(x => x.naturalidade)
                .MaximumLength(40)
                .WithMessage("O campo 'naturalidade' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.naturalidade));

            RuleFor(x => x.tempo_residencia)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tempo_residencia));

            RuleFor(x => x.renda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.renda));

            RuleFor(x => x.numero_compl_rua_cliente)
                .MaximumLength(10)
                .WithMessage("O campo 'numero_compl_rua_cliente' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_compl_rua_cliente));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.tipo_pessoa)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_pessoa' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.tipo_pessoa));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.aceita_programa_fidelidade)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.aceita_programa_fidelidade));
        }
    }
}
