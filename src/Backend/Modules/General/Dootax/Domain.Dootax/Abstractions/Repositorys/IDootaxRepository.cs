using Domain.Dootax.Entities;

namespace Domain.Dootax.Interfaces.Repositorys
{
    public interface IDootaxRepository
    {
        public Task<IEnumerable<XML>> GetXMLs();
        public Task InsertSendFilesSuccessfulLog(string cnpjcpf, string documento, string serie, string chavenfe);
    }
}
