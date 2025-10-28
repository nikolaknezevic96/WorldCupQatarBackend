using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.Interfaces;
using QatarWorldCupBackend.DTO;
using System.Configuration;
using System.Text;

namespace QatarWorldCupBackend.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;

        }


        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<bool> DeleteAllUsersAsync()

        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var users = await _context.Users.ToListAsync();
                _context.Users.RemoveRange(users);



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

        public async Task<User> Register(User user, string password)
        {
            if (await _context.Users.AnyAsync(x => x.Username == user.Username))
                throw new ApplicationException("Username is already taken");

            user.PasswordHash = CreatePasswordHash(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private string CreatePasswordHash(string password)
        {
            var key = Encoding.UTF8.GetBytes("adhdhdhdhdhjdhdhdhdhdsjladadsjadsjkaKJKVJKFJKFDJKDFJKFDJKJKDASKADSJKLADSJHDSFJDSFJ");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(key))
            {
                return Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var key = Encoding.UTF8.GetBytes("adhdhdhdhdhjdhdhdhdhdsjladadsjadsjkaKJKVJKFJKFDJKDFJKFDJKJKDASKADSJKLADSJHDSFJDSFJ");
            using (var hmac = new System.Security.Cryptography.HMACSHA512(key))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var computedHashString = Convert.ToBase64String(computedHash);
                Console.WriteLine($"Computed Hash: {computedHashString}");
                Console.WriteLine($"Stored Hash: {storedHash}");
                return computedHashString == storedHash;
            }
        }
    }
}
