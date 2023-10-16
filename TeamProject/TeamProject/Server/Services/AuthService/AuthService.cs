using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TeamProject.Server.Data;
using TeamProject.Shared.Response;
using TeamProject.Shared.User;

namespace TeamProject.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ApplicationDbContext dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPaswd)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "NotFound" };
            }
            CreatePasswdHash(newPaswd, out byte[] passwdHash, out byte[] passwdSalt);
            //adding 32+ character to passwd and after it hash it for security
            user.PasswordSalt = passwdSalt;
            user.PasswordHash = passwdHash;
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true, Message = "Password changed" };
        }

        public async Task<User> GetUserByMail(string email)
        {
            var response = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == email);
            return response!;
        }

        public string GetUserEmail()
            => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

        public int GetUserId()
            => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        private void CreatePasswdHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash =
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public async Task<ServiceResponse<string>> Login(string email, string passwd)
        {
            var response = new ServiceResponse<string>();
            var usr = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (usr == null)
            {
                response.Success = false;
                response.Message = "NotFound";
            }
            else if (!VerifyPasswordHash(passwd, usr.PasswordHash, usr.PasswordSalt))
            {
                response.Success = false;
                response.Message = "NotMatched";
            }
            else
            {
                response.Data = CreateToken(usr);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserCheck(user.Email))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "NotFound"
                };
            }

            CreatePasswdHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse<int>
            {
                Data = user.Id,
                Message = "You have been registerd "
            };
        }

        public async Task<bool> UserCheck(string email)
        {
            if (await _dbContext.Users.AnyAsync(usr => usr.Email == email))
            {
                return true;
            }
            return false;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var secretKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
