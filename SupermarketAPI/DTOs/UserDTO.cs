namespace SupermarketAPI.DTOs
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public string UserRole { get; set; } = null!;
    } 
    
    public class UserRequest
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public string UserRole { get; set; } = null!;
    }
}
