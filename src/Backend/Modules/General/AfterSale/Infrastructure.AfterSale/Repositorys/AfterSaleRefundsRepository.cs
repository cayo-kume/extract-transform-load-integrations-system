using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AfterSale.Repositorys
{
    public class AfterSaleRefundsRepository : IAfterSaleRefundsRepository
    {
        private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public AfterSaleRefundsRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

        public Task<bool> InsertIntoAfterSaleRefunds()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertIntoAfterSaleRefundsActions()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertIntoAfterSaleRefundsBanks()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertIntoAfterSaleRefundsStatus()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertIntoAfterSaleRefundsTypes()
        {
            throw new NotImplementedException();
        }
    }
}
