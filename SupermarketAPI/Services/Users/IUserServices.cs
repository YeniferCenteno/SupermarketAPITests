using SupermarketAPI.DTOs;

namespace SupermarketAPI.Services.Users
{
    public interface IUserServices
    {
        Task<int> PostUser(UserRequest user);
        Task<List<UserResponse>> GetUsers();
        Task<UserResponse> GetUser(int UserId);
        Task<int> PutUser(int usertId, UserRequest user);
        Task<int> DeleteUser(int userId);
        Task<UserResponse> Login(UserRequest user);


    }
}
