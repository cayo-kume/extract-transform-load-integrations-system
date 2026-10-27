using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosValidator : AbstractValidator<LinxProdutos>
    {
        public LinxProdutosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_produto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_produto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.cod_barra)
                .MaximumLength(20)
                .WithMessage("O campo 'cod_barra' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_barra));

            RuleFor(x => x.nome)
                .MaximumLength(250)
                .WithMessage("O campo 'nome' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome));

            RuleFor(x => x.ncm)
                .MaximumLength(20)
                .WithMessage("O campo 'ncm' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ncm));

            RuleFor(x => x.cest)
                .MaximumLength(10)
                .WithMessage("O campo 'cest' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cest));

            RuleFor(x => x.referencia)
                .MaximumLength(20)
                .WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referencia));

            RuleFor(x => x.cod_auxiliar)
                .MaximumLength(40)
                .WithMessage("O campo 'cod_auxiliar' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_auxiliar));

            RuleFor(x => x.unidade)
                .MaximumLength(50)
                .WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unidade));

            RuleFor(x => x.desc_cor)
                .MaximumLength(30)
                .WithMessage("O campo 'desc_cor' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_cor));

            RuleFor(x => x.desc_tamanho)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_tamanho' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_tamanho));

            RuleFor(x => x.desc_setor)
                .MaximumLength(30)
                .WithMessage("O campo 'desc_setor' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_setor));

            RuleFor(x => x.desc_linha)
                .MaximumLength(30)
                .WithMessage("O campo 'desc_linha' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_linha));

            RuleFor(x => x.desc_marca)
                .MaximumLength(30)
                .WithMessage("O campo 'desc_marca' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_marca));

            RuleFor(x => x.desc_colecao)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_colecao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_colecao));

            RuleFor(x => x.dt_update)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_update));

            RuleFor(x => x.cod_fornecedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_fornecedor));

            RuleFor(x => x.desativado)
                .MaximumLength(1)
                .WithMessage("O campo 'desativado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desativado));

            RuleFor(x => x.desc_espessura)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_espessura' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_espessura));

            RuleFor(x => x.id_espessura)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_espessura));

            RuleFor(x => x.desc_classificacao)
                .MaximumLength(50)
                .WithMessage("O campo 'desc_classificacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_classificacao));

            RuleFor(x => x.id_classificacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_classificacao));

            RuleFor(x => x.origem_mercadoria)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.origem_mercadoria));

            RuleFor(x => x.peso_liquido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.peso_liquido));

            RuleFor(x => x.peso_bruto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.peso_bruto));

            RuleFor(x => x.id_cor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cor));

            RuleFor(x => x.id_tamanho)
                .MaximumLength(5)
                .WithMessage("O campo 'id_tamanho' deve ter no máximo 5 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id_tamanho));

            RuleFor(x => x.id_setor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_setor));

            RuleFor(x => x.id_linha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_linha));

            RuleFor(x => x.id_marca)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_marca));

            RuleFor(x => x.id_colecao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_colecao));

            RuleFor(x => x.dt_inclusao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_inclusao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.fator_conversao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.fator_conversao));

            RuleFor(x => x.codigo_integracao_ws)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));

            RuleFor(x => x.codigo_integracao_reshop)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_integracao_reshop' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_integracao_reshop));

            RuleFor(x => x.id_produtos_opticos_tipo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_tipo));

            RuleFor(x => x.id_sped_tipo_item)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_sped_tipo_item));

            RuleFor(x => x.componente)
                .MaximumLength(1)
                .WithMessage("O campo 'componente' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.componente));

            RuleFor(x => x.altura_para_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.altura_para_frete));

            RuleFor(x => x.largura_para_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.largura_para_frete));

            RuleFor(x => x.comprimento_para_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.comprimento_para_frete));

            RuleFor(x => x.codigoproduto_io)
                .MaximumLength(50)
                .WithMessage("O campo 'codigoproduto_io' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_io));

            RuleFor(x => x.cod_comprador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_comprador));

            RuleFor(x => x.codigo_integracao_oms)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_integracao_oms' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_integracao_oms));

            RuleFor(x => x.obrigatorio_identificacao_cliente)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.obrigatorio_identificacao_cliente));

            RuleFor(x => x.descricao_basica)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao_basica' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_basica));

            RuleFor(x => x.curva)
                .MaximumLength(1)
                .WithMessage("O campo 'curva' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.curva));

            RuleFor(x => x.tributa_fcp)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.tributa_fcp));

            RuleFor(x => x.observacao)
                .MaximumLength(1000)
                .WithMessage("O campo 'observacao' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacao));

            RuleFor(x => x.codigo_adicional_reshop)
                .MaximumLength(100)
                .WithMessage("O campo 'codigo_adicional_reshop' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_adicional_reshop));

            RuleFor(x => x.brinde)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.brinde));

            RuleFor(x => x.exige_controle)
                .MaximumLength(1)
                .WithMessage("O campo 'exige_controle' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.exige_controle));

            RuleFor(x => x.multiplo_venda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.multiplo_venda));

            RuleFor(x => x.unidade_compra)
                .MaximumLength(50)
                .WithMessage("O campo 'unidade_compra' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unidade_compra));
        }
    }
}
