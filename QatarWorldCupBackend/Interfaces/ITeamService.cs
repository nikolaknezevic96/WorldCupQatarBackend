using QatarWorldCupBackend.DTO;

namespace QatarWorldCupBackend.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDTO>> GetAllTeamsAsync();
        Task<TeamDTO> GetTeamByIdAsync(int id);
        Task<TeamDTO> CreateTeamAsync(TeamDTO teamDTO);
        Task<bool> UpdateTeamAsync(int id, TeamDTO teamDTO);
        Task<bool> DeleteTeamAsync(int id);
        Task<bool> AddTeamToGroupAsync(int teamId, int groupId);
        Task<bool> RemoveTeamFromGroupAsync(int teamId);
        Task<IEnumerable<TeamDTO>> GetTeamByGroupIdAsync(int groupId);

        Task<bool> DeleteAllTeamsAsync();
    }
}
