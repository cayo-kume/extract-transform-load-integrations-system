using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesRedeValidator : AbstractValidator<LinxClientesRede>
    {
        public LinxClientesRedeValidator()
        {
            RuleFor(x => x.id_clientes_rede).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_clientes_rede));
            RuleFor(x => x.doc_cliente).MaximumLength(14).WithMessage("O campo 'doc_cliente' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.doc_cliente));
            RuleFor(x => x.nome_razao_social).MaximumLength(60).WithMessage("O campo 'nome_razao_social' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_razao_social));
            RuleFor(x => x.data_cadastro).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_cadastro));
            RuleFor(x => x.pf_pj).MaximumLength(1).WithMessage("O campo 'pf_pj' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.pf_pj));
            RuleFor(x => x.tipo).MaximumLength(1).WithMessage("O campo 'tipo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo));
            RuleFor(x => x.endereco).MaximumLength(250).WithMessage("O campo 'endereco' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco));
            RuleFor(x => x.cidade).MaximumLength(40).WithMessage("O campo 'cidade' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.cidade));
            RuleFor(x => x.uf).MaximumLength(2).WithMessage("O campo 'uf' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.uf));
            RuleFor(x => x.pais).MaximumLength(80).WithMessage("O campo 'pais' deve ter no máximo 80 caracteres.").When(x => !string.IsNullOrEmpty(x.pais));
            RuleFor(x => x.id_estado_civil).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_estado_civil));
            RuleFor(x => x.compras_a_prazo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.compras_a_prazo));
            RuleFor(x => x.aceita_boleto_bancario).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.aceita_boleto_bancario));
            RuleFor(x => x.nome_fantasia).MaximumLength(60).WithMessage("O campo 'nome_fantasia' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_fantasia));
            RuleFor(x => x.numero_endereco).MaximumLength(20).WithMessage("O campo 'numero_endereco' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_endereco));
            RuleFor(x => x.complemento).MaximumLength(60).WithMessage("O campo 'complemento' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.complemento));
            RuleFor(x => x.bairro).MaximumLength(60).WithMessage("O campo 'bairro' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.bairro));
            RuleFor(x => x.cep).MaximumLength(9).WithMessage("O campo 'cep' deve ter no máximo 9 caracteres.").When(x => !string.IsNullOrEmpty(x.cep));
            RuleFor(x => x.telefone).MaximumLength(20).WithMessage("O campo 'telefone' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.telefone));
            RuleFor(x => x.celular).MaximumLength(20).WithMessage("O campo 'celular' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.celular));
            RuleFor(x => x.fax).MaximumLength(50).WithMessage("O campo 'fax' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.fax));
            RuleFor(x => x.email).MaximumLength(50).WithMessage("O campo 'email' deve ter no máximo 50 caracteres.").EmailAddress().When(x => !string.IsNullOrEmpty(x.email));
            RuleFor(x => x.site).MaximumLength(50).WithMessage("O campo 'site' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.site));
            RuleFor(x => x.data_nascimento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_nascimento));
            RuleFor(x => x.naturalidade).MaximumLength(40).WithMessage("O campo 'naturalidade' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.naturalidade));
            RuleFor(x => x.sexo).MaximumLength(1).WithMessage("O campo 'sexo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sexo));
            RuleFor(x => x.nome_pai).MaximumLength(60).WithMessage("O campo 'nome_pai' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_pai));
            RuleFor(x => x.nome_mae).MaximumLength(60).WithMessage("O campo 'nome_mae' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_mae));
            RuleFor(x => x.identidade_cliente).MaximumLength(20).WithMessage("O campo 'identidade_cliente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.identidade_cliente));
            RuleFor(x => x.data_emissao_identidade_cliente).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_emissao_identidade_cliente));
            RuleFor(x => x.uf_emissao_identidade_cliente).MaximumLength(2).WithMessage("O campo 'uf_emissao_identidade_cliente' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.uf_emissao_identidade_cliente));
            RuleFor(x => x.orgao_emissao_identidade_cliente).MaximumLength(10).WithMessage("O campo 'orgao_emissao_identidade_cliente' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.orgao_emissao_identidade_cliente));
            RuleFor(x => x.observacao).MaximumLength(500).WithMessage("O campo 'observacao' deve ter no máximo 500 caracteres.").When(x => !string.IsNullOrEmpty(x.observacao));
            RuleFor(x => x.nome_empresa_titular).MaximumLength(60).WithMessage("O campo 'nome_empresa_titular' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_empresa_titular));
            RuleFor(x => x.telefone_empresa_titular).MaximumLength(20).WithMessage("O campo 'telefone_empresa_titular' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.telefone_empresa_titular));
            RuleFor(x => x.funcao_titular).MaximumLength(40).WithMessage("O campo 'funcao_titular' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.funcao_titular));
            RuleFor(x => x.tempo_servico_titular).MaximumLength(20).WithMessage("O campo 'tempo_servico_titular' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.tempo_servico_titular));
            RuleFor(x => x.renda_titular).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.renda_titular));
            RuleFor(x => x.inscricao_estadual).MaximumLength(20).WithMessage("O campo 'inscricao_estadual' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.inscricao_estadual));
            RuleFor(x => x.inscricao_municipal).MaximumLength(20).WithMessage("O campo 'inscricao_municipal' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.inscricao_municipal));
            RuleFor(x => x.cliente_optante_simples_municipal).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.cliente_optante_simples_municipal));
            RuleFor(x => x.cliente_optante_simples_federal).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.cliente_optante_simples_federal));
            RuleFor(x => x.limite_compras).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.limite_compras));
            RuleFor(x => x.cartao_fidelidade).MaximumLength(20).WithMessage("O campo 'cartao_fidelidade' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cartao_fidelidade));
            RuleFor(x => x.desabilitado).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.desabilitado));
            RuleFor(x => x.motivo_bloqueio).MaximumLength(50).WithMessage("O campo 'motivo_bloqueio' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.motivo_bloqueio));
            RuleFor(x => x.portal_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal_origem));
            RuleFor(x => x.empresa_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa_origem));
            RuleFor(x => x.cod_cliente_portal_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente_portal_origem));
            RuleFor(x => x.codigo_ws).MaximumLength(14).WithMessage("O campo 'codigo_ws' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
            RuleFor(x => x.integrado_ws).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.integrado_ws));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
