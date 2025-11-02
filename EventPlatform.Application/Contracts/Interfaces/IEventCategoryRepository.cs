using EventPlatform.Domain.Entities;

namespace EventPlatform.Domain.Interfaces
{
    public interface IEventCategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetAllEventCategories();
    }
}
