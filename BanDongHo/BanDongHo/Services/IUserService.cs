using BanDongHo.DTOs;

namespace BanDongHo.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetByIdAsync(string id);
        Task<bool> UpdateUserAsync(string id, UserDTO dto);
        Task<bool> DeleteUserAsync(string id);
    }
}
