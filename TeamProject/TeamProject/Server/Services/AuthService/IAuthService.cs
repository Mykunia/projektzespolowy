using TeamProject.Shared.Response;
using TeamProject.Shared.User;

namespace TeamProject.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<User> GetUserByMail(string email);
        Task<bool> UserCheck(string email);
        string GetUserEmail();
        int GetUserId();
    }
}
