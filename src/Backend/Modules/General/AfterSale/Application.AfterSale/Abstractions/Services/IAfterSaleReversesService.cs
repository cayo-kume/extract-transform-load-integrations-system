using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Abstractions.Services
{
    public interface IAfterSaleReversesService
    {
        public Task<bool?> GetStatus();
        public Task<bool?> GetTransportations();
        public Task<bool?> GetReverses();
        public Task<bool?> GetReverse();
        public Task<bool?> GetCourierAttributes();
        
        public Task<bool?> PostApproveReverseById(long id);
        public Task<bool?> PostReverseInvoiceById(long id);
        
        public Task<bool?> PutReciveProductsById(long id);
        public Task<bool?> PutReturnedInvoiceById(long id);
    }
}
