using Microsoft.EntityFrameworkCore;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services.Interfaces;
using TestTaskApi.Domain.Entities;
using TestTaskApi.Infrastructure.Data;

namespace TestTaskApi.Application.Services
{
    

    public class UtilityService : IUtilityService
    {
        private readonly ApplicationDbContext _context;

        public UtilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UtilityDto>> GetAllUtilitiesAsync()
        {
            return await _context.Utilities
                .Select(u => new UtilityDto { Id = u.Id, Name = u.Name, BasePrice = u.BasePrice })
                .ToListAsync();
        }

        public async Task<UtilityDto> CreateUtilityAsync(CreateUtilityDto dto)
        {
            var utility = new Utility { Name = dto.Name, BasePrice = dto.BasePrice };
            _context.Utilities.Add(utility);
            await _context.SaveChangesAsync();

            return new UtilityDto { Id = utility.Id, Name = utility.Name, BasePrice = utility.BasePrice };
        }

        public async Task<UtilityDto?> UpdateUtilityAsync(int id, UpdateUtilityDto dto)
        {
            var utility = await _context.Utilities.FindAsync(id);
            if (utility is null) return null;

            if (dto.Name is not null) utility.Name = dto.Name;
            if (dto.BasePrice.HasValue) utility.BasePrice = dto.BasePrice.Value;

            await _context.SaveChangesAsync();
            return new UtilityDto { Id = utility.Id, Name = utility.Name, BasePrice = utility.BasePrice };
        }

        public async Task<bool> DeleteUtilityAsync(int id)
        {
            var utility = await _context.Utilities.FindAsync(id);
            if (utility is null) return false;

            _context.Utilities.Remove(utility);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}