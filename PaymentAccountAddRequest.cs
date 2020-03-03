using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests
{
	public class PaymentAccountAddRequest
	{
		public int UserId { get; set; }
		public string AccountId { get; set; }
		public int PaymentTypeId { get; set; }
        public string StripePublicKey { get; set; }
        public string StripeSecretKey { get; set; }


    }
}
