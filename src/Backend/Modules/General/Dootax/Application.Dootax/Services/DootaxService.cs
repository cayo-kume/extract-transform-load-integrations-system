using Application.Dootax.Interfaces.Services;
using Domain.Dootax.Interfaces.Apis;
using Domain.Dootax.Interfaces.Repositorys;

namespace Application.Dootax.Services
{
    public class DootaxService : IDootaxService
    {
        private readonly IAPICall _apiCall;
        private readonly IDootaxRepository _dootaxRepository;

        public DootaxService(IDootaxRepository dootaxRepository, IAPICall apiCall) =>
            (_dootaxRepository, _apiCall) = (dootaxRepository, apiCall);

        public async Task<bool> ImportFilesUpload()
        {
            var xmls = await _dootaxRepository.GetXMLs();

            if (xmls.Count() > 0)
            {
                foreach (var xml in xmls)
                {
                    var result = await _apiCall.PostAsync(xml);

                    if (result != null)
                        await _dootaxRepository.InsertSendFilesSuccessfulLog(xml.CNPJCPF, xml.Documento, xml.Serie, xml.ChaveNfe);
                    else
                        continue;
                }
            }

            return true;
        }
    }
}
