using Domain.Dootax.Entities;

namespace Domain.Dootax.Interfaces.Apis
{
    public interface IAPICall
    {
        public Task<XML> PostAsync(XML xml);
    }
}
