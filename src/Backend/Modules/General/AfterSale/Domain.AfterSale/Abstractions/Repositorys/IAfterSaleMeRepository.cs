using Domain.AfterSale.Entities.Reads.Me;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Abstractions.Repositorys
{
    public interface IAfterSaleMeRepository
    {
        public Task<bool> BulkInsertIntoTableAfterSaleEcommerce(List<Ecommerce> data, Guid? parentExecutionGUID);
    }
}
