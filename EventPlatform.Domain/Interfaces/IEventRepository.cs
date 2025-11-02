using EventPlatform.Domain.Entities;

namespace EventPlatform.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task CreateAsync(Event e);
    }
}
