using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Server.Services.AuthService;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;

        public AddressService(ApplicationDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new ServiceResponse<Address>();
            var getAddress = (await GetAddress()).Data;
            if(getAddress == null)
            {
                address.UserId = _authService.GetUserId();
                _dbContext.Addresses.Add(address);
                response.Data = address;
            }
            else
            {
                //We cen use automapper but it's slowing down app
                getAddress.FirstName = address.FirstName;
                getAddress.LastName = address.LastName;
                getAddress.State = address.State;
                getAddress.Country = address.Country;
                getAddress.City = address.City;
                getAddress.PostalCode = address.PostalCode;
                getAddress.Street = address.Street;
                response.Data = getAddress;
            }

            await _dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<Address>> GetAddress()
        {
            int userId = _authService.GetUserId();
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.UserId == userId);
            return new ServiceResponse<Address>
            {
                Data = address
            };
        }
    }
}
