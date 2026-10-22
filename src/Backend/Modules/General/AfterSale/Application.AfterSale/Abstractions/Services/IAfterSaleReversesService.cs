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
        public Task<bool?> GetReverseById(string user_name, string doc_company, long id);
        public Task<bool?> GetCourierAttributes();

        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Approve.Response.Root> PostApproveReverseById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.Approve.Request.Root request);
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root> PostReverseInvoiceById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Request.Root request);
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root> PostReverseInvoiceXmlById(string user_name, string doc_company, long id, byte[] xml, string fileName);
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Response.Root> PostAdministrativeRecordById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Request.Root request);

        public Task<Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Response.Root> PutReciveProductsById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Request.Root request);
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Response.Root> PutReturnedInvoiceById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Request.Root request);
    }
}
