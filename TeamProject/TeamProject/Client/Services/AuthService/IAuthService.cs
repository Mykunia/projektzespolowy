using TeamProject.Shared.Response;
using TeamProject.Shared.User;

namespace TeamProject.Client.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> ChangePassword(UserChangePassword req);
        Task<ServiceResponse<int>> Register(UserRegister req);
        Task<ServiceResponse<string>> Login(UserLogin req);
        Task<bool> IsUserAuth();
    }
}
