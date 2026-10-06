using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesFornecValidator : AbstractValidator<LinxClientesFornec>
    {
        public LinxClientesFornecValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.razao_cliente).MaximumLength(60).WithMessage("O campo 'razao_cliente' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.razao_cliente));
            RuleFor(x => x.nome_cliente).MaximumLength(60).WithMessage("O campo 'nome_cliente' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_cliente));
            RuleFor(x => x.doc_cliente).MaximumLength(14).WithMessage("O campo 'doc_cliente' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.doc_cliente));
            RuleFor(x => x.tipo_cliente).MaximumLength(1).WithMessage("O campo 'tipo_cliente' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_cliente));
            RuleFor(x => x.endereco_cliente).MaximumLength(250).WithMessage("O campo 'endereco_cliente' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_cliente));
            RuleFor(x => x.numero_rua_cliente).MaximumLength(20).WithMessage("O campo 'numero_rua_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_rua_cliente));
            RuleFor(x => x.complement_end_cli).MaximumLength(60).WithMessage("O campo 'complement_end_cli' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.complement_end_cli));
            RuleFor(x => x.bairro_cliente).MaximumLength(60).WithMessage("O campo 'bairro_cliente' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.bairro_cliente));
            RuleFor(x => x.cep_cliente).MaximumLength(9).WithMessage("O campo 'cep_cliente' deve ter no máximo 9 caracteres.").When(x => !string.IsNullOrEmpty(x.cep_cliente));
            RuleFor(x => x.cidade_cliente).MaximumLength(40).WithMessage("O campo 'cidade_cliente' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.cidade_cliente));
            RuleFor(x => x.uf_cliente).MaximumLength(20).WithMessage("O campo 'uf_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.uf_cliente));
            RuleFor(x => x.pais).MaximumLength(80).WithMessage("O campo 'pais' deve ter no máximo 80 caracteres.").When(x => !string.IsNullOrEmpty(x.pais));
            RuleFor(x => x.fone_cliente).MaximumLength(20).WithMessage("O campo 'fone_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.fone_cliente));
            RuleFor(x => x.email_cliente).MaximumLength(50).WithMessage("O campo 'email_cliente' deve ter no máximo 50 caracteres.").EmailAddress().When(x => !string.IsNullOrEmpty(x.email_cliente));
            RuleFor(x => x.sexo).MaximumLength(1).WithMessage("O campo 'sexo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sexo));
            RuleFor(x => x.data_cadastro).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_cadastro));
            RuleFor(x => x.data_nascimento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_nascimento));
            RuleFor(x => x.cel_cliente).MaximumLength(20).WithMessage("O campo 'cel_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cel_cliente));
            RuleFor(x => x.ativo).MaximumLength(1).WithMessage("O campo 'ativo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.dt_update).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_update));
            RuleFor(x => x.inscricao_estadual).MaximumLength(20).WithMessage("O campo 'inscricao_estadual' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.inscricao_estadual));
            RuleFor(x => x.incricao_municipal).MaximumLength(20).WithMessage("O campo 'incricao_municipal' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.incricao_municipal));
            RuleFor(x => x.identidade_cliente).MaximumLength(20).WithMessage("O campo 'identidade_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.identidade_cliente));
            RuleFor(x => x.cartao_fidelidade).MaximumLength(20).WithMessage("O campo 'cartao_fidelidade' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cartao_fidelidade));
            RuleFor(x => x.cod_ibge_municipio).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_ibge_municipio));
            RuleFor(x => x.classe_cliente).MaximumLength(83).WithMessage("O campo 'classe_cliente' deve ter no máximo 83 caracteres.").When(x => !string.IsNullOrEmpty(x.classe_cliente));
            RuleFor(x => x.matricula_conveniado).MaximumLength(20).WithMessage("O campo 'matricula_conveniado' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.matricula_conveniado));
            RuleFor(x => x.tipo_cadastro).MaximumLength(1).WithMessage("O campo 'tipo_cadastro' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_cadastro));
            RuleFor(x => x.empresa_cadastro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa_cadastro));
            RuleFor(x => x.id_estado_civil).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_estado_civil));
            RuleFor(x => x.fax_cliente).MaximumLength(50).WithMessage("O campo 'fax_cliente' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.fax_cliente));
            RuleFor(x => x.site_cliente).MaximumLength(50).WithMessage("O campo 'site_cliente' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.site_cliente));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.cliente_anonimo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.cliente_anonimo));
            RuleFor(x => x.limite_compras).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.limite_compras));
            RuleFor(x => x.codigo_ws).MaximumLength(100).WithMessage("O campo 'codigo_ws' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
            RuleFor(x => x.limite_credito_compra).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.limite_credito_compra));
            RuleFor(x => x.id_classe_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classe_fiscal));
            RuleFor(x => x.mae).MaximumLength(60).WithMessage("O campo 'mae' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.mae));
        }
    }
}
