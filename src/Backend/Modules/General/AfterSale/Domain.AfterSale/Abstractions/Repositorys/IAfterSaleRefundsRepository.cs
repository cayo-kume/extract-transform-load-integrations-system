using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Abstractions.Repositorys
{
    public interface IAfterSaleRefundsRepository
    {
        public Task<bool> InsertIntoAfterSaleRefundsStatus();
        public Task<bool> InsertIntoAfterSaleRefundsActions();
        public Task<bool> InsertIntoAfterSaleRefundsBanks();
        public Task<bool> InsertIntoAfterSaleRefundsTypes();
        public Task<bool> InsertIntoAfterSaleRefunds();
    }
}
