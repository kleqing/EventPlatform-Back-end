using EventPlatform.Domain.Interfaces;
using EventPlatform.Infrastructure.Data;

namespace EventPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // Add methods to interact with the User entity in the database
}