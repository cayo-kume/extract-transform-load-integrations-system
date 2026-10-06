using Application.Dootax.Interfaces.Handlers.Commands;
using Domain.Core.Abstractions.Repositorys;
using Domain.Dootax.Entities;
using Domain.Dootax.Interfaces.Repositorys;

namespace Infrastructure.Dootax.Repositorys
{
    public class DootaxRepository : IDootaxRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly IDootaxCommandHandler _dootaxCommandHandler;

        public DootaxRepository(IIntegrationsCoreRepository coreRepository, IDootaxCommandHandler dootaxCommandHandler) =>
            (_coreRepository, _dootaxCommandHandler) = (coreRepository, dootaxCommandHandler);

        public async Task<IEnumerable<XML>> GetXMLs()
        {
            var sql = _dootaxCommandHandler.CreateGetXMLsQuery();
            return await _coreRepository.GetRecords<XML>(sql);
        }

        public async Task InsertSendFilesSuccessfulLog(string cnpjcpf, string documento, string serie, string chavenfe)
        {
            var sql = _dootaxCommandHandler.CreateSuccessfulLogQuery();

            await _coreRepository.ExecuteCommand(sql, new
            {
                cnpj_emp = cnpjcpf,
                documento = documento,
                serie = serie,
                chave_nfe = chavenfe,
                dt_insert = DateTime.Now
            });
        }
    }
}
