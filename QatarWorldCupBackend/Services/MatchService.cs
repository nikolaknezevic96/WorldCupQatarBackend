using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using QatarWorldCupBackend.Data;
using System.IO;

namespace QatarWorldCupBackend.Services
{
    public class MatchService : IMatchService
    {
        private readonly DataContext _context;

        public MatchService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchDTO>> GetAllMatchesAsync()
        {
            var matches = await _context.Matches
                .Include(u => u.Team1)
                .Include(u => u.Team2)
                .Include(u => u.Stadium)
                .ToListAsync();
            return matches.Adapt<List<MatchDTO>>();
        }

        public async Task<MatchDTO> GetMatchByIdAsync(int id)
        {
            var match = await _context.Matches
                .Include(u => u.Team1)
                .Include(u => u.Team2)
                .Include(u => u.Stadium)
                .FirstOrDefaultAsync(u => u.Id == id);
            return match?.Adapt<MatchDTO>();
        }

        public async Task<MatchDTO> CreateMatchAsync(MatchDTO matchDTO)
        {
            if (matchDTO.Team1Id == matchDTO.Team2Id)
                throw new InvalidOperationException("A team cannot play against itself. Please select two different teams.");

            if (!await AreTeamsInSameGroup(matchDTO.Team1Id, matchDTO.Team2Id))
                throw new InvalidOperationException("Teams are not in the same group. Matches can only be scheduled between teams in the same group.");

            if (await HaveTeamsPlayedBefore(matchDTO.Team1Id, matchDTO.Team2Id))
                throw new InvalidOperationException("These teams have already played against each other in the group stage.");

            // Skip stadion availability and time checks if the match is forfeited
            if (!matchDTO.Team1Forfeited && !matchDTO.Team2Forfeited)
            {
                if (!IsValidTime(matchDTO.StartDateTime))
                    throw new InvalidOperationException("Match start time is invalid. Matches must start between 14:00 and 23:00.");

                if (!await IsStadiumAvailable(matchDTO.StadiumId, matchDTO.StartDateTime))
                    throw new InvalidOperationException("The selected stadium is not available at the chosen time.");

                if (await IsTeamAlreadyPlaying(matchDTO.Team1Id, matchDTO.StartDateTime) ||
                    await IsTeamAlreadyPlaying(matchDTO.Team2Id, matchDTO.StartDateTime))
                    throw new InvalidOperationException("One of the teams is already scheduled for another match at the same time.");
            }

            var match = matchDTO.Adapt<Match>();
            match.Forfeited = matchDTO.Team1Forfeited || matchDTO.Team2Forfeited;
            if (matchDTO.Team1Forfeited)
            {
                match.Team1GoalsScored = 0;
                match.Team2GoalsScored = 3;
            }
            else if (matchDTO.Team2Forfeited)
            {
                match.Team1GoalsScored = 3;
                match.Team2GoalsScored = 0;
            }

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            if (match.Forfeited)
            {                
                await UpdateTeamStats(match.Team1Id, match.Team1GoalsScored, match.Team2GoalsScored);
                await UpdateTeamStats(match.Team2Id, match.Team2GoalsScored, match.Team1GoalsScored);
            }

            return match.Adapt<MatchDTO>();
        }

        private async Task<bool> IsTeamAlreadyPlaying(int teamId, DateTime matchTime)
        {
            // Setting the time window to 2 hours before and after the match start time
            DateTime startTime = matchTime.AddHours(-2);
            DateTime endTime = matchTime.AddHours(2);

            // Check if the team has any match scheduled within this time window
            return await _context.Matches.AnyAsync(u =>
                (u.Team1Id == teamId || u.Team2Id == teamId) &&
                u.StartDateTime.Date == matchTime.Date &&
                u.StartDateTime >= startTime &&
                u.StartDateTime <= endTime);
        }

        private async Task UpdateTeamStats(int teamId, int? scoredGoals, int? concededGoals)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null && scoredGoals.HasValue && concededGoals.HasValue)
            {
                team.NumGoalsScored += scoredGoals.Value;
                team.NumGoalsConceded += concededGoals.Value;

                if (scoredGoals > concededGoals)
                {
                    team.NumPoints += 3;
                    team.NumWins += 1;
                }
                else if (scoredGoals == concededGoals)
                {
                    team.NumPoints += 1;
                    team.NumDraws += 1;
                }
                else
                {
                    team.NumLosses += 1;
                }
                await _context.SaveChangesAsync();
            }
        }

        private void UpdateTeamStats1(Team team, int? scoredGoals, int? concededGoals)
        {
            if (scoredGoals.HasValue && concededGoals.HasValue)
            {
                team.NumGoalsScored += scoredGoals.Value;
                team.NumGoalsConceded += concededGoals.Value;

                if (scoredGoals > concededGoals)
                {
                    team.NumPoints += 3;
                    team.NumWins += 1;
                }
                else if (scoredGoals == concededGoals)
                {
                    team.NumPoints += 1;
                    team.NumDraws += 1;
                }
                else
                {
                    team.NumLosses += 1;
                }
            }
        }

        private async Task<bool> HaveTeamsPlayedBefore(int team1Id, int team2Id)
        {
            return await _context.Matches.AnyAsync(u =>
                (u.Team1Id == team1Id && u.Team2Id == team2Id) || (u.Team1Id == team2Id && u.Team2Id == team1Id));
        }

        public async Task<bool> UpdateMatchAsync(int id, MatchDTO matchDTO)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return false;
            matchDTO.Adapt(match);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return false;
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }
        

        public async Task<bool> SetMatchResultAsync(int matchId, int team1GoalsScored, int team2GoalsScored)
        {
            var match = await _context.Matches.Include(u => u.Team1).Include(u => u.Team2).FirstOrDefaultAsync(u => u.Id == matchId);
            if (match == null)
            {
                return false;
            }

            // Check if the match has already started
            if (match.StartDateTime > DateTime.Now)
            {
                // Throw an exception if trying to set result before match start time
                throw new InvalidOperationException("Nije moguće postaviti rezultat pre vremena početka matches.");
            }
            
            match.Team1GoalsScored = team1GoalsScored;
            match.Team2GoalsScored = team2GoalsScored;
            
            if (match.Team1 != null && match.Team2 != null)
            {
                UpdateTeamStats1(match.Team1, team1GoalsScored, team2GoalsScored);
                UpdateTeamStats1(match.Team2, team2GoalsScored, team1GoalsScored);
            }

            await _context.SaveChangesAsync(); 
            return true;
        }


        private bool IsValidTime(DateTime time)
        {
            return time.TimeOfDay >= new TimeSpan(14, 0, 0) && time.TimeOfDay <= new TimeSpan(23, 0, 0);
        }

        private async Task<bool> IsStadiumAvailable(int? stadionId, DateTime time)
        {
            if (stadionId == null) return false;
            return !await _context.Matches.AnyAsync(u =>
        u.StadiumId == stadionId &&
        u.StartDateTime.Date == time.Date &&
        u.StartDateTime >= time.AddHours(-4) &&
        u.StartDateTime <= time.AddHours(4));
        }

        private async Task<bool> AreTeamsInSameGroup(int team1Id, int team2Id)
        {
            var team1 = await _context.Teams.FindAsync(team1Id);
            var team2 = await _context.Teams.FindAsync(team2Id);
            return team1 != null && team2 != null && team1.GroupId == team2.GroupId;
        }
        
        public async Task<IEnumerable<MatchDTO>> GetMatchesByGroupIdAsync(int groupId)
        {
            var matches = await _context.Matches.Include(u => u.Team1).Include(u => u.Team2).Include(u => u.Stadium).Where(u => u.Team1.GroupId == groupId || u.Team2.GroupId == groupId).ToListAsync();

            return matches.Adapt<List<MatchDTO>>();
        }

        public async Task<bool> DeleteAllMatchesAsync()
        {
            var allMatches = await _context.Matches.ToListAsync();
            if (!allMatches.Any()) return false;

            _context.Matches.RemoveRange(allMatches);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
