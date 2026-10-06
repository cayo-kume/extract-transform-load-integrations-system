using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Abstractions.Services
{
    public interface IAfterSaleRefundsService
    {
        public Task<bool?> GetStatus();
        public Task<bool?> GetActions();
        public Task<bool?> GetBanks();
        public Task<bool?> GetTypes();
        public Task<bool?> GetRefunds();
        public Task<bool?> GetRefundById(long id);
        public Task<bool?> PostApproveRefundById(long id);
        public Task<bool?> PostCancelRefundById(long id);
        public Task<bool?> PostPayRefundById(long id);
        public Task<bool?> PostChangeTotalAmountRefundById(long id);
    }
}
