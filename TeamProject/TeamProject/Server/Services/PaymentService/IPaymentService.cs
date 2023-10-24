using TeamProject.Shared.Response;
using Stripe.Checkout;

namespace TeamProject.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request);
        Task<Session> CreateCheckoutSession();
    }
}
