using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Infrastructure.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Registration> AddAsync(Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task AddRangeAsync(List<Registration> registrations)
        {
            await _context.Registrations.AddRangeAsync(registrations);
            await _context.SaveChangesAsync();
        }
    }
}
