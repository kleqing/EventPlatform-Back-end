using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Contracts.Interfaces
{
    public interface IEventCategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetAllEventCategories();
    }
}
