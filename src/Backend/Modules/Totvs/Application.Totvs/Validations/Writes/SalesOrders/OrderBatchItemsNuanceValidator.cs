using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo da sugestão de itens de lote pelo nuance (400 se inválido): só a chave
    /// </summary>
    public class OrderBatchItemsNuanceValidator : AbstractValidator<OrderBatchItemsNuance>
    {
        public OrderBatchItemsNuanceValidator()
        {
            Include(new OrderKeyValidator());
        }
    }
}
