namespace Bookstore.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int userId, string email, string name, string role);
    }
}
