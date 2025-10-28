using Domain.Model;

namespace QatarWorldCupBackend.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Register(User user, string password);

        Task<bool> DeleteAllUsersAsync();
    }
}
