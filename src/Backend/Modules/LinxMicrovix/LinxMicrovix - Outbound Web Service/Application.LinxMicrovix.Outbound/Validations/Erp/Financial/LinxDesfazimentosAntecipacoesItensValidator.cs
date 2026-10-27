using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxDesfazimentosAntecipacoesItensValidator : AbstractValidator<LinxDesfazimentosAntecipacoesItens>
    {
        public LinxDesfazimentosAntecipacoesItensValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_desfazimentos_antecipacoes_itens)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_desfazimentos_antecipacoes_itens' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_desfazimentos_antecipacoes)
                .MaximumLength(1000)
                .WithMessage("O campo 'id_desfazimentos_antecipacoes' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id_desfazimentos_antecipacoes));

            RuleFor(x => x.numero_sequencial)
                .MaximumLength(1000)
                .WithMessage("O campo 'numero_sequencial' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_sequencial));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.preco)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
