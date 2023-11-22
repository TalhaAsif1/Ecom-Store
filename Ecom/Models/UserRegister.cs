namespace Ecom.Models
{
    public class UserRegister
    {
        public int UserRegisterId { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
