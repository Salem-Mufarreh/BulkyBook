using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings Options { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }
        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            Options = options.Value;
        }
        public IBraintreeGateway CreateGeteway()
        {
            return new BraintreeGateway(Options.Enviroment, Options.MerchantID, Options.PublicKey, Options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if(BraintreeGateway == null)
            {
                BraintreeGateway = CreateGeteway();
            }
            return BraintreeGateway;
        }
    }
}
