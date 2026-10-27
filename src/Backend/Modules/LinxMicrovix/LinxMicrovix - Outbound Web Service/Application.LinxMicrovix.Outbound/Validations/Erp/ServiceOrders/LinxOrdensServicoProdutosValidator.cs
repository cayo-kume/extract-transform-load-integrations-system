using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxOrdensServicoProdutosValidator : AbstractValidator<LinxOrdensServicoProdutos>
    {
        public LinxOrdensServicoProdutosValidator()
        {
            RuleFor(x => x.numero_os)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_os));

            RuleFor(x => x.cod_produto_serv)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto_serv));

            RuleFor(x => x.id_ordservprod)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_ordservprod' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
