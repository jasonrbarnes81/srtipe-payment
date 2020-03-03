[HttpPost("schedulePayment")]
        public ActionResult<ItemResponse<object>> SubscriptionSchedule(SubscriptionItems item)
        {
            int iCode = 200;
            BaseResponse response = null;
            List<object> subscriptionResponses = null;

            try
            {
                subscriptionResponses = _pService.CreateSubscriptionPayments(item);
                if(subscriptionResponses == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemsResponse<object> { Items = subscriptionResponses };
                }
            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }

            return StatusCode(iCode, response);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<ItemResponse<PaymentIntent>>> Payment(long amount, string currency, string paymentType, string email)
        {
            int iCode = 200;
            BaseResponse response = null;
            PaymentIntent paymentItem = null;

            try
            {
                paymentItem = await _pService.MakePayment(amount, currency, paymentType, email);
                if (paymentItem == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<PaymentIntent> { Item = paymentItem };
                }

            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }

            return StatusCode(iCode, response);
        }

        [HttpPost("processing")]
        [Obsolete]
        public  ActionResult<ItemsResponse<object>> AccountInfo(string accountId)
        {
            int iCode = 200;
            BaseResponse response = null;
            OAuthToken Item = null;
            PaymentAccountAddRequest add = null;

            try
            {
                Item =  _pService.StripeAccountInfo(accountId);
                if (Item == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<object> { Item = Item };
                }
                int userId = _authService.GetCurrentUserId();
                add = new PaymentAccountAddRequest();
                add.AccountId = Item.StripeUserId;
                add.StripePublicKey = Item.StripePublishableKey;
                add.StripeSecretKey = Item.AccessToken;
                add.PaymentTypeId = 1;
                add.UserId = userId;

                CreatePaymentAccount(add);



            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }

            return StatusCode(iCode, response);
        }

        [HttpGet("link")]
        public ActionResult<ItemResponse<LoginLink>> AccountLink(string accountId)
        {
            int iCode = 200;
            BaseResponse response = null;
            LoginLink link;

            try
            {
                link = _pService.StripeAccountLink(accountId);
                if (link == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<LoginLink> { Item = link };
                }
            }
            catch(Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }
            return StatusCode(iCode, response);
        }

        [HttpGet("charge")]
        public ActionResult<ItemResponse<Charge>> ChargeCard(string token, string key, int amount)
        {
            int iCode = 200;
            BaseResponse response = null;
            Charge charge;

            try
            {
                charge = _pService.StripeCharge(token,key,amount);
                if (charge == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Charge> { Item = charge };
                }
            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }
            return StatusCode(iCode, response);
        }

        [HttpGet("transfer")]
        public ActionResult<ItemResponse<Charge>> Stripetransfer(string accountId, int amount, string circleId)
        {
            int iCode = 200;
            BaseResponse response = null;
            Transfer transfer;

            try
            {
                transfer = _pService.StripeTransfer(accountId, amount,circleId);
                if (transfer == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Transfer> { Item = transfer };
                }
            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }
            return StatusCode(iCode, response);
        }

        [HttpPost("verify")]
        public ActionResult<ItemResponse<Person>> StripeVerify(IFormFile file, string accountId)
        {
            int iCode = 200;
            BaseResponse response = null;
            Person account;

            try
            {
                account = _pService.AccountVerification(file, accountId);
                if (account == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<Person> { Item = account };
                }
            }
            catch (Exception e)
            {
                iCode = 500;
                response = new ErrorResponse(e.Message);
            }
            return StatusCode(iCode, response);
        }
    
