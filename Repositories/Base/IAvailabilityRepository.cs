using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IAvailabilityRepository
    {
        Task addRangeAsync(List<availability> entities);
    }
}
