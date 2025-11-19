using EventPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Interfaces
{
    public interface IRegistrationRepository
    {
        // Tạo 1 lượt đăng ký
        Task<Registration> AddAsync(Registration registration);

        // Tạo nhiều lượt đăng ký cùng lúc (Bulk Insert) - Dùng khi user mua nhiều vé
        Task AddRangeAsync(List<Registration> registrations);
    }
}
