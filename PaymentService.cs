public List<object> CreateSubscriptionPayments(SubscriptionItems item)
        {
            List<object> list = null;
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey;

            Plan plan = CreatePlan(item);

            Customer customer = CreateCustomer(item);

            SubscriptionSchedule schedule = CreateSubSchedule(item, customer, plan);
            if (list == null)
            {
                list = new List<object>();
            }
            list.Add(schedule);
            list.Add(customer);
            list.Add(plan);



            return list;
        }

        public Plan CreatePlan(SubscriptionItems item)
        {
            string optionInterval = null;
            int optionIntervalCount = 1;
            PlanCreateOptions planOptions = null;
            if (item.Interval == "Bi-Weekly")
            {
                optionInterval = "week";
                optionIntervalCount = 2;
            }
            else
            { optionInterval = item.Interval; }

            planOptions = new PlanCreateOptions
            {
                Product = new PlanProductCreateOptions { Name = item.CircleName },
                Amount = item.Amount,
                Currency = "usd",

                Interval = optionInterval,
                IntervalCount = optionIntervalCount

            };

            PlanService planService = new PlanService();
            Plan plan = planService.Create(planOptions);

            return plan;
        }

        public Customer CreateCustomer(SubscriptionItems item)
        {
            string customerName = item.FirstName + " " + item.LastName;
            CustomerCreateOptions customerOptions = null;
            customerOptions = new CustomerCreateOptions
            {
                Email = item.Email,
                Name = customerName,
                Address = new AddressOptions()
                {
                    Line1 = item.LineOne,
                    Line2 = null,
                    City = item.City,
                    State = item.State,
                    PostalCode = item.Zip,
                    Country = "US"
                }
            };

            CustomerService customerService = new CustomerService();
            Customer customer = customerService.Create(customerOptions);
            return customer;
        }

        public SubscriptionSchedule CreateSubSchedule(SubscriptionItems item, Customer customer, Plan plan)
        {
            SubscriptionScheduleCreateOptions subscriptionOptions = null;
            subscriptionOptions = new SubscriptionScheduleCreateOptions
            {
                Customer = customer.Id,
                StartDate = item.StartDate,
                EndBehavior = "cancel",
                Phases = new List<SubscriptionSchedulePhaseOptions>
            {
                new SubscriptionSchedulePhaseOptions
                {
                     Plans = new List<SubscriptionSchedulePhaseItemOptions>
                     {
                         new SubscriptionSchedulePhaseItemOptions
                         {
                              Plan = plan.Id,
                              Quantity = 1,
                         },
                     },
                    Iterations = item.Participants,
                },

            }
            };
            SubscriptionScheduleService subService = new SubscriptionScheduleService();
            SubscriptionSchedule schedule = subService.Create(subscriptionOptions);
            return schedule;
        }

        public Task<PaymentIntent> MakePayment(long amount, string currency, string paymentType, string email)
        {
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey; 
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { paymentType },
                ReceiptEmail = email
            };

            var service = new PaymentIntentService();
            //service.CreateAsync(options);

            return service.CreateAsync(options);
        }


        public OAuthToken StripeAccountInfo(string accountId)
        {
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey;
            var options = new OAuthTokenCreateOptions
            {
                GrantType = "authorization_code",
                Code = accountId,
            };

            var service = new OAuthTokenService();
            var response = service.Create(options);
            

            return response;
        }
    
        public LoginLink StripeAccountLink(string accountId)
        {
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey;
            var service = new LoginLinkService();
            var link = service.Create(accountId);
            return link;
        }

        public Charge StripeCharge(string token, string key, int amount)
        {
            StripeConfiguration.ApiKey = key; //_credentials.Value.ApiKey;
            var options = new ChargeCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                Description = "Saving Circle Charge",
                Source = token,
            };

            var service = new ChargeService();
            var cardCharge = service.Create(options);


            return cardCharge;
        }

        public Transfer StripeTransfer(string accountId, int amount, string circleId)
        {
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey;

            var options = new TransferCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                Destination = accountId,
                TransferGroup = circleId,
            };

            var service = new TransferService();
            return service.Create(options);
        }


        public Person AccountVerification(IFormFile file, string accountId)
        {
            StripeConfiguration.ApiKey = _credentials.Value.ApiKey;
            Stripe.File upload = null;

            var accountService = new AccountService();
            Account account = accountService.Get(accountId);

            using (Stream stream = file.OpenReadStream())
            {
                var options = new FileCreateOptions
                {
                    File = stream,
                    Purpose = FilePurpose.IdentityDocument,
                };

                var fileService = new Stripe.FileService();
                upload = fileService.Create(options);
            }

            var accountOptions = new PersonUpdateOptions
            {
               
                    Verification = new PersonVerificationOptions
                    {
                        Document = new PersonVerificationDocumentOptions
                        {
                            Front = upload.Id,
                        },
                    },
                
            };

            var service = new PersonService();
            return service.Update(accountId,null, accountOptions);
        }
