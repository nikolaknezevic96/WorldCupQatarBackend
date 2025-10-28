using QatarWorldCupBackend.DTO;

namespace QatarWorldCupBackend.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDTO>> GetAllMatchesAsync();
        Task<MatchDTO> GetMatchByIdAsync(int id);
        Task<MatchDTO> CreateMatchAsync(MatchDTO matchDTO);
        Task<bool> UpdateMatchAsync(int id, MatchDTO matchDTO);
        Task<bool> DeleteMatchAsync(int id);
        Task<bool> SetMatchResultAsync(int matchId, int team1GoalsScored, int team2GoalsScored);
        Task<IEnumerable<MatchDTO>> GetMatchesByGroupIdAsync(int groupId);
        Task<bool> DeleteAllMatchesAsync();

    }
}
