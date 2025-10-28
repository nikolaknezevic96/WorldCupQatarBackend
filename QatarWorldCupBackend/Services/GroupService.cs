using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using System.Text.RegularExpressions;

namespace QatarWorldCupBackend.Services
{
    public class GroupService : IGroupService
    {
        private readonly DataContext _context;

        public GroupService(DataContext context)
        {
            _context = context;
        }    

        public async Task<IEnumerable<GroupDTO>> GetAllGroupsAsync()
        {
            var groups = await _context.Groups.ToListAsync();
            return groups.Adapt<List<GroupDTO>>();
        }

        public async Task<GroupDTO> GetGroupByIdAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            return group?.Adapt<GroupDTO>();
        }

        public async Task<GroupDTO> CreateGroupAsync(GroupDTO groupDTO)
        {
            var group = groupDTO.Adapt<Domain.Model.Group>();
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group.Adapt<GroupDTO>();
        }

        public async Task<bool> UpdateGroupAsync(int id, GroupDTO groupDTO)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return false;
            groupDTO.Adapt(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return false;
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AddTeamToGroupAsync(int groupId, int teamId)
        {
            var group = await _context.Groups.Include(g => g.Teams).FirstOrDefaultAsync(g => g.Id == groupId);
            var team = await _context.Teams.FindAsync(teamId);
            if (group == null || team == null || group.Teams.Count >= 4) return false;

            group.Teams.Add(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTeamFromGroupAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null || team.GroupId == null) return false;

            team.Group = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {                
                var matches = await _context.Matches.ToListAsync();
                _context.Matches.RemoveRange(matches);
                
                var teams = await _context.Teams.ToListAsync();
                _context.Teams.RemoveRange(teams);
                
                var groups = await _context.Groups.ToListAsync();
                _context.Groups.RemoveRange(groups);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}

