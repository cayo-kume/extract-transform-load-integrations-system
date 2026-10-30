using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxListagemBalancoValidator : AbstractValidator<LinxListagemBalanco>
    {
        public LinxListagemBalancoValidator()
        {
            RuleFor(x => x.id_balanco)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_balanco' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.data)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data));

            RuleFor(x => x.nome_arquivo)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_arquivo' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_arquivo));

            RuleFor(x => x.processado)
                .MaximumLength(1)
                .WithMessage("O campo 'processado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.processado));

            RuleFor(x => x.data_ultimo_processamento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_ultimo_processamento));

            RuleFor(x => x.qtde_produtos)
                .MaximumLength(1000)
                .WithMessage("O campo 'qtde_produtos' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.qtde_produtos));

            RuleFor(x => x.qtde_itens)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_itens));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
