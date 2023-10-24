using Stripe;
using Stripe.Checkout;
using TeamProject.Server.Services.AuthService;
using TeamProject.Server.Services.CartService;
using TeamProject.Server.Services.OrderServices;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        const string secret = "do_zmiany";
        public PaymentService(IAuthService authService, IOrderService orderService, ICartService cartService)
        {
            StripeConfiguration.ApiKey = "do_pobrani_stripe";
            _authService = authService;
            _orderService = orderService;
            _cartService = cartService;
        }
        public async Task<Session> CreateCheckoutSession()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            var lineItems = new List<SessionLineItemOptions>();
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                        Images = new List<string> { product.PictureURL }
                    }
                },
                Quantity = product.Quantity
            }));

            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                ShippingAddressCollection =
                    new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> { "US" }
                    },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "Url_localhost/order-success",
                CancelUrl = "Url_localhost/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                        json,
                        request.Headers["Stripe-Signature"],
                        secret
                    );

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authService.GetUserByMail(session.CustomerEmail);
                    await _orderService.PlaceOrder(user.Id);
                }

                return new ServiceResponse<bool> { Data = true };
            }
            catch (StripeException e)
            {
                return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
            }
        }
    }
}
