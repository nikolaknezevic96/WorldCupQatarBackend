using Domain.Model;
using QatarWorldCupBackend.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QatarWorldCupBackend.Interfaces
{
    public interface IStadiumService
    {
        Task<IEnumerable<StadiumDTO>> GetAllStadiumsAsync();
        Task<StadiumDTO> GetStadiumByIdAsync(int id);
        Task<StadiumDTO> CreateStadiumAsync(StadiumDTO stadiumDTO);
        Task<bool> UpdateStadiumAsync(int id, StadiumDTO stadiumDTO);
        Task<bool> DeleteStadiumAsync(int id);
    }

}