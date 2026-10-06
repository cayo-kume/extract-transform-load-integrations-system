using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Abstractions.Services
{
    public interface IAfterSaleMeService
    {
        public Task<bool?> GetMe();
    }
}
