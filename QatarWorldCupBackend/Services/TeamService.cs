using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;

namespace QatarWorldCupBackend.Services
{
    public class TeamService : ITeamService
    {
        private readonly DataContext _context;

        public TeamService(DataContext context)
        {
            _context = context;
        }       

        public async Task<IEnumerable<TeamDTO>> GetAllTeamsAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Adapt<List<TeamDTO>>();
        }

        public async Task<TeamDTO> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            return team?.Adapt<TeamDTO>();
        }

        public async Task<TeamDTO> CreateTeamAsync(TeamDTO teamDTO)
        {
            bool teamExists = await _context.Teams.AnyAsync(t => t.TeamName == teamDTO.TeamName);
            if (teamExists)
            {
                throw new InvalidOperationException("Team sa isteam imenom već postoji u bazi.");
            }

            var team = teamDTO.Adapt<Team>();

            // Provera da li je dodeljena group i validacija kapaciteta
            if (team.GroupId.HasValue)
            {
                var group = await _context.Groups.Include(g => g.Teams).FirstOrDefaultAsync(g => g.Id == team.GroupId.Value);
                if (group == null)
                {
                    throw new InvalidOperationException("Specifikovana group ne postoji.");
                }
                if (group.Teams.Count >= 4)
                {
                    throw new InvalidOperationException("Group već ima maksimalan broj teamova (4).");
                }
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team.Adapt<TeamDTO>();
        }

        public async Task<bool> UpdateTeamAsync(int id, TeamDTO teamDTO)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return false;
            teamDTO.Adapt(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return false;
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTeamToGroupAsync(int teamId, int groupId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            var group = await _context.Groups.Include(g => g.Teams).FirstOrDefaultAsync(g => g.Id == groupId);
            if (team == null || group == null || group.Teams.Count >= 4) return false;
            team.Group = group;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTeamFromGroupAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null || team.GroupId == null) return false;
            team.GroupId = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeamDTO>> GetTeamByGroupIdAsync(int groupId)
        {
            return await _context.Teams
      .Where(t => t.GroupId == groupId)
      .Select(t => new TeamDTO
      {
          Id = t.Id,
          TeamName = t.TeamName,
          NumGoalsScored = t.NumGoalsScored,
          NumDraws = t.NumDraws,
          NumWins = t.NumWins,
          NumPoints = t.NumPoints,
          NumLosses = t.NumLosses,
          NumGoalsConceded = t.NumGoalsConceded,
          GroupId = t.GroupId,


      }).ToListAsync();
        }


        public async Task<bool> DeleteAllTeamsAsync()
        {
            var allTeams = await _context.Teams.ToListAsync();
            if (!allTeams.Any()) return false;

            _context.Teams.RemoveRange(allTeams);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}

