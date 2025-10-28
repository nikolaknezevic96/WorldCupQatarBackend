using QatarWorldCupBackend.DTO;

namespace QatarWorldCupBackend.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDTO>> GetAllGroupsAsync();
        Task<GroupDTO> GetGroupByIdAsync(int id);
        Task<GroupDTO> CreateGroupAsync(GroupDTO grupaDTO);
        Task<bool> UpdateGroupAsync(int id, GroupDTO grupaDTO);
        Task<bool> AddTeamToGroupAsync(int grupaId, int timId);
        Task<bool> RemoveTeamFromGroupAsync(int timId);
        Task<bool> DeleteGroupAsync(int id);
        Task<bool> DeleteAllDataAsync();
    }
}
