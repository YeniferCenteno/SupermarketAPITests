using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupermarketAPI.DTOs;
using SupermarketAPI.Models;

namespace SupermarketAPI.Services.Users
{
    public class UserServices : IUserServices
    {
        private readonly SupermarketDbContext _db;
        private readonly IMapper _mapper;

        public UserServices(SupermarketDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> DeleteUser(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return -1;

            _db.Users.Remove(user);

            return await _db.SaveChangesAsync();
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            var users = await _db.Users.ToListAsync();
            var userList = _mapper.Map<List<User>, List<UserResponse>>(users);
            return userList;
        }

        public async Task<UserResponse> GetUser(int UserId)
        {
            var user = await _db.Users.FindAsync(UserId);
            var userResponse = _mapper.Map<User, UserResponse>(user);
            return userResponse;
        }

        public async Task<int> PostUser(UserRequest user)
        {
            var userRequest = _mapper.Map<UserRequest, User>(user);
            await _db.Users.AddAsync(userRequest);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> PutUser(int usertId, UserRequest user)
        {
            var entity = await _db.Users.FindAsync(usertId);
            if (entity == null)
                return -1;

            entity.Username = user.Username;
            entity.UserPassword = user.UserPassword;
            entity.UserRole = user.UserRole;

            _db.Users.Update(entity);

            return await _db.SaveChangesAsync();
        }

        public async Task<UserResponse> Login(UserRequest user)
        {
            var userEntity = await _db.Users.FirstOrDefaultAsync(
                    o=>o.Username == user.Username &&
                    o.UserPassword == user.UserPassword
                );

            var userResponse = _mapper.Map<User, UserResponse>(userEntity);
            return userResponse;
        }
    }
}
