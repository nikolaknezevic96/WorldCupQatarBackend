using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QatarWorldCupBackend.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly DataContext _context;

        public StadiumService(DataContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<StadiumDTO>> GetAllStadiumsAsync()
        {
            var stadiums = await _context.Stadiums.ToListAsync();
            return stadiums.Adapt<List<StadiumDTO>>();
        }

        public async Task<StadiumDTO> GetStadiumByIdAsync(int id)
        {
            var stadium = await _context.Stadiums.FindAsync(id);
            return stadium?.Adapt<StadiumDTO>();
        }

        public async Task<StadiumDTO> CreateStadiumAsync(StadiumDTO stadiumDTO)
        {
            var stadium = stadiumDTO.Adapt<Stadium>();
            _context.Stadiums.Add(stadium);
            await _context.SaveChangesAsync();
            return stadium.Adapt<StadiumDTO>();
        }

        public async Task<bool> UpdateStadiumAsync(int id, StadiumDTO stadiumDTO)
        {
            var stadium = await _context.Stadiums.FindAsync(id);
            if (stadium == null) return false;
            stadiumDTO.Adapt(stadium);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStadiumAsync(int id)
        {
            var stadium = await _context.Stadiums.FindAsync(id);
            if (stadium == null) return false;
            _context.Stadiums.Remove(stadium);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}