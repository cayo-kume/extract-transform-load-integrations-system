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
        public Task<bool?> GetRefund();
        public Task<bool?> GetRefundById(string user_name, string doc_company, long id);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root> PostApproveRefundById(string user_name, string doc_company, long id);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root> PostCancelRefundById(string user_name, string doc_company, long id);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root> PostPayRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request, byte[] file, string fileName, string fileContentType);
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root> PostChangeTotalAmountRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request);
    }
}
