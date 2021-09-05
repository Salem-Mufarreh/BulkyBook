using Braintree;

namespace BulkyBook.Utility
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGeteway();
        IBraintreeGateway GetGateway();

    }
}
