using Domain.Core.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Validations.LinxCommerce
{
    public class B2CConsultaClientesValidator : AbstractValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes>
    {
        public B2CConsultaClientesValidator()
        {
            RuleFor(x => x.cod_cliente_b2c)
                .MustBeValidInt32();

            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32();

            RuleFor(x => x.doc_cliente)
                .MaximumLength(14)
                .WithMessage("O campo 'doc_cliente' deve ter no máximo 14 caracteres.");

            RuleFor(x => x.nm_cliente)
                .MaximumLength(60)
                .WithMessage("O campo 'nm_cliente' deve ter no máximo 60 caracteres.");

            RuleFor(x => x.nm_mae)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_mae' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.nm_pai)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_pai' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.nm_conjuge)
                .MaximumLength(50)
                .WithMessage("O campo 'nm_conjuge' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.dt_cadastro)
                .MustBeValidDateTime();

            RuleFor(x => x.dt_nasc_cliente)
                .MustBeValidDateTime();

            RuleFor(x => x.end_cliente)
                .MaximumLength(250)
                .WithMessage("O campo 'end_cliente' deve ter no máximo 250 caracteres.");

            RuleFor(x => x.complemento_end_cliente)
                .MaximumLength(50)
                .WithMessage("O campo 'complemento_end_cliente' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.nr_rua_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'nr_rua_cliente' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.bairro_cliente)
                .MaximumLength(60)
                .WithMessage("O campo 'bairro_cliente' deve ter no máximo 60 caracteres.");

            RuleFor(x => x.cep_cliente)
                .MaximumLength(9)
                .WithMessage("O campo 'cep_cliente' deve ter no máximo 9 caracteres.");

            RuleFor(x => x.cidade_cliente)
                .MaximumLength(40)
                .WithMessage("O campo 'cidade_cliente' deve ter no máximo 40 caracteres.");

            RuleFor(x => x.uf_cliente)
                .MaximumLength(2)
                .WithMessage("O campo 'uf_cliente' deve ter no máximo 2 caracteres.");

            RuleFor(x => x.fone_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'fone_cliente' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.fone_comercial)
                .MaximumLength(20)
                .WithMessage("O campo 'fone_comercial' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.cel_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'cel_cliente' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.email_cliente)
                .MaximumLength(50)
                .WithMessage("O campo 'email_cliente' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.rg_cliente)
                .MaximumLength(20)
                .WithMessage("O campo 'rg_cliente' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.rg_orgao_emissor)
                .MaximumLength(7)
                .WithMessage("O campo 'rg_orgao_emissor' deve ter no máximo 7 caracteres.");

            RuleFor(x => x.estado_civil_cliente)
                .MustBeValidInt32();

            RuleFor(x => x.empresa_cliente)
                .MaximumLength(30)
                .WithMessage("O campo 'empresa_cliente' deve ter no máximo 30 caracteres.");

            RuleFor(x => x.cargo_cliente)
                .MaximumLength(30)
                .WithMessage("O campo 'cargo_cliente' deve ter no máximo 30 caracteres.");

            RuleFor(x => x.sexo_cliente)
                .MaximumLength(1)
                .WithMessage("O campo 'sexo_cliente' deve ter no máximo 1 caractere.");

            RuleFor(x => x.dt_update)
                .MustBeValidDateTime();

            RuleFor(x => x.ativo)
                .MustBeValidInt32();

            RuleFor(x => x.receber_email)
                .MustBeValidBoolean();

            RuleFor(x => x.dt_expedicao_rg)
                .MustBeValidDateTime();

            RuleFor(x => x.naturalidade)
                .MaximumLength(40)
                .WithMessage("O campo 'naturalidade' deve ter no máximo 40 caracteres.");

            RuleFor(x => x.tempo_residencia)
                .MustBeValidInt32();

            RuleFor(x => x.renda)
                .MustBeValidDecimal();

            RuleFor(x => x.numero_compl_rua_cliente)
                .MaximumLength(10)
                .WithMessage("O campo 'numero_compl_rua_cliente' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.timestamp)
                .MustBeValidInt64();

            RuleFor(x => x.tipo_pessoa)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_pessoa' deve ter no máximo 1 caractere.");

            RuleFor(x => x.portal)
                .MustBeValidInt32();

            RuleFor(x => x.aceita_programa_fidelidade)
                .MustBeValidBoolean();
        }
    }
}
