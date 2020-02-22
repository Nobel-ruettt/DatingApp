using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return null;

            if (!VerifyPassword(password, user.PassWordHash, user.PassWordSalt)) return null;

            return user;
        }

        private bool VerifyPassword(string password, byte[] passWordHash, byte[] passWordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passWordSalt))
            {
                var computedhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0; i<computedhash.Length;i++)
                {
                    if (computedhash[i] != passWordHash[i]) return false;
                }

            }
            return true;
        }


        public async Task<User> Register(User user, string password)
        {
            byte[] PassWordHash, PassWordSalt;
            CreatePassWordHash(password, out PassWordHash, out PassWordSalt);

            user.PassWordHash = PassWordHash;
            user.PassWordSalt = PassWordSalt;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePassWordHash(string password, out byte[] passWordHash, out byte[] passWordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passWordSalt = hmac.Key;
                passWordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
                
        }

        public async Task<bool> UserExists(string username)
        {
            var exist = await _context.Users.AnyAsync(x => x.UserName == username);
            return exist;
        }
    }
}
