using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;

namespace WatchAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Retrieving all users");

            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDTO>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
                result.Add(userDto);
            }
            return result;
        }

        public async Task<UserDTO?> GetByIdAsync(string id)
        {
            _logger.LogInformation("Fetching user by id {id}", id);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {id} not found", id);
                return null;
            }

            var dto = _mapper.Map<UserDTO>(user);
            dto.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";

            _logger.LogInformation("Found user {username} with id {id}", dto.UserName, id);

            return dto;
        }

        public async Task<bool> UpdateUserAsync(string id, UserDTO dto)
        {
            try
            {
                _logger.LogInformation("Updating user {id}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Cannot update user {id}: not found", id);
                    return false;
                }

                user.Email = dto.Email;
                user.UserName = dto.UserName;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user {id}. Errors: {errors}",
                        id,
                        string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    _logger.LogInformation("Role {role} does not exist. Creating...", dto.Role);
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                }

                await _userManager.AddToRoleAsync(user, dto.Role);

                _logger.LogInformation("User {id} updated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting user {id}", id);
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Cannot delete user {id}: not found", id);
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user {id}. Errors: {errors}",
                        id,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                _logger.LogInformation("User {id} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}
