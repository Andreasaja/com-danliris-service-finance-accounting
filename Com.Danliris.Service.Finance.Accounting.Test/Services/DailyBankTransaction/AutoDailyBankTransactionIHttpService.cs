﻿using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    class AutoDailyBankTransactionIHttpService : IHttpClientService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            if(url.Contains("master/account-banks/"))
            {
                string id = url.Substring(url.LastIndexOf('/') + 1);

                var defaultresponse = new APIDefaultResponse<AccountBank>()
                {
                    data = new AccountBank()
                    {
                        Currency = new Currency()
                    }
                };

                if (id == "2")
                    defaultresponse.data.Currency.Code = "IDR";

                var result = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(defaultresponse))
                };

                return Task.FromResult(result);
            }

            else if (url.Contains("master/garment-currencies/single-by-code-date"))
            {
                var defaultresponse = new APIDefaultResponse<GarmentCurrency>()
                {
                    data = new GarmentCurrency()
                    {
                        Code = "USD",
                        Rate = 15500.00
                    }
                };

                var result = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(defaultresponse))
                };

                return Task.FromResult(result);
            }

            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}
