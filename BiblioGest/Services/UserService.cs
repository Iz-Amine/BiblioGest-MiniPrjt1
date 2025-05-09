using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BiblioGest.Data;
using BiblioGest.Models;

namespace BiblioGest.Services
{
    public class UserService
    {
        private readonly BibliothequeContext _context;

        public UserService()
        {
            _context = App.DbContext;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                // Check if username or email already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    throw new Exception("Ce nom d'utilisateur est déjà utilisé.");
                }

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    throw new Exception("Cet email est déjà utilisé.");
                }

                user.DateCreation = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                    return false;

                // Check if username or email is being changed and if it's already used
                if (existingUser.Username != user.Username)
                {
                    if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                        throw new Exception("Ce nom d'utilisateur est déjà utilisé.");
                }

                if (existingUser.Email != user.Email)
                {
                    if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                        throw new Exception("Cet email est déjà utilisé.");
                }

                user.DateModification = DateTime.Now;
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.PasswordHash = newPasswordHash;
                user.DateModification = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllUsersAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Users
                .Where(u => u.Username.ToLower().Contains(searchTerm) ||
                           u.Nom.ToLower().Contains(searchTerm) ||
                           u.Prenom.ToLower().Contains(searchTerm) ||
                           u.Email.ToLower().Contains(searchTerm))
                .ToListAsync();
        }
    }
} 