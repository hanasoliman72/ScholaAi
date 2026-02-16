using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IAvailabilityRepository
    {
        Task AddRangeAsync(List<Availability> entities);
    }
}
