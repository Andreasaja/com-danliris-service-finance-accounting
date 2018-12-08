﻿using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionAcceptance;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionReport;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionVerification;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.PurchasingDispositionExpedition
{
    public class PurchasingDispositionExpeditionServiceTest
    {
        private const string ENTITY = "PurchasingDispositionExpeditions";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private PurchasingDispositionExpeditionDataUtil _dataUtil(PurchasingDispositionExpeditionService service)
        {
            return new PurchasingDispositionExpeditionDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        private Mock<IServiceProvider> GetServiceProviderWrongHttpClient()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientFailTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_Verification()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var Response = service.Read(1, 25, "{}", null, null, "{'verificationFilter':''}");
            Assert.NotNull(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionViewModel vm = _dataUtil(service).GetNewViewModel();

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            PurchasingDispositionExpeditionViewModel vm = new PurchasingDispositionExpeditionViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(newModel.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Post_Acceptance_Verification()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "VERIFICATION",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };

            var response = await service.PurchasingDispositionAcceptance(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Fail_Post_Acceptance_Verification()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "VERIFICATION",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };
            await Assert.ThrowsAnyAsync<Exception>(() => service.PurchasingDispositionAcceptance(data));

        }

        [Fact]
        public async void Should_Success_Post_Acceptance_Cashier()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "CASHIER",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };

            var response = await service.PurchasingDispositionAcceptance(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async void Should_Success_Delete_Acceptance_Verification()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "VERIFICATION",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };
            var acceptedResponse = await service.PurchasingDispositionAcceptance(data);
            var newModel = await service.ReadByIdAsync(model.Id);
            var deleteResponse = await service.DeletePurchasingDispositionAcceptance(newModel.Id);
            Assert.NotEqual(0, deleteResponse);
        }

        [Fact]
        public async void Should_Success_Delete_Acceptance_Cashier()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "CASHIER",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };
            var acceptedResponse = await service.PurchasingDispositionAcceptance(data);
            var newModel = await service.ReadByIdAsync(model.Id);
            var deleteResponse = await service.DeletePurchasingDispositionAcceptance(newModel.Id);
            Assert.NotEqual(0, deleteResponse);
        }

        [Fact]
        public async void Should_Fail_Delete_Empty_Id()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var deleteResponse = await service.DeletePurchasingDispositionAcceptance(-1);
            Assert.Equal(0, deleteResponse);
        }

        [Fact]
        public void Should_Success_Validate_Purchasing_Disposition_Acceptance_Vm()
        {
            PurchasingDispositionAcceptanceViewModel nullVM = new PurchasingDispositionAcceptanceViewModel();
            nullVM.PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>();

            Assert.True(nullVM.Validate(null).Count() > 0);
            PurchasingDispositionAcceptanceViewModel vm = new PurchasingDispositionAcceptanceViewModel();
            vm.PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
            {
                new PurchasingDispositionAcceptanceItemViewModel()
                {
                    DispositionNo = "DispositonNo",
                    Id = 1
                }
            };

            Assert.True(vm.Validate(null).Count() == 0);

        }

        [Fact]
        public async void Should_Success_Post_Disposition_Verification_Update_Cashier()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionVerificationViewModel data = new PurchasingDispositionVerificationViewModel()
            {
                DispositionNo = model.DispositionNo,
                Id = model.Id,
                Reason = "Reason",
                SubmitPosition = ExpeditionPosition.SEND_TO_CASHIER_DIVISION,
                VerifyDate = DateTimeOffset.UtcNow
            };
            var response = await service.PurchasingDispositionVerification(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async void Should_Success_Post_Disposition_Verification_Create_Purchasing()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionVerificationViewModel data = new PurchasingDispositionVerificationViewModel()
            {
                DispositionNo = model.DispositionNo,
                Id = 0,
                Reason = "Reason",
                SubmitPosition = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION,
                VerifyDate = DateTimeOffset.UtcNow
            };
            var response = await service.PurchasingDispositionVerification(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Fail_Post_Disposition_Verification()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionExpeditionModel model = await _dataUtil(service).GetTestData();

            PurchasingDispositionVerificationViewModel data = new PurchasingDispositionVerificationViewModel()
            {
                DispositionNo = model.DispositionNo,
                Id = 0,
                Reason = "Reason",
                SubmitPosition = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION,
                VerifyDate = DateTimeOffset.UtcNow
            };
            await Assert.ThrowsAnyAsync<Exception>(() => service.PurchasingDispositionVerification(data));

        }

        [Fact]
        public async Task Should_Fail_Delete_Acceptance()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            PurchasingDispositionAcceptanceViewModel data = new PurchasingDispositionAcceptanceViewModel()
            {
                Role = "CASHIER",
                PurchasingDispositionExpedition = new List<PurchasingDispositionAcceptanceItemViewModel>()
                {
                    new PurchasingDispositionAcceptanceItemViewModel()
                    {
                        DispositionNo  = model.DispositionNo,
                        Id = model.Id
                    }
                }
            };
            var acceptedResponse = await service.PurchasingDispositionAcceptance(data);
            var newModel = await service.ReadByIdAsync(model.Id);
            service = new PurchasingDispositionExpeditionService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeletePurchasingDispositionAcceptance(newModel.Id));

        }

        [Fact]
        public void Should_No_Error_Validate_VM_Verification_Disposition()
        {
            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PurchasingDispositionVerificationViewModel vm = new PurchasingDispositionVerificationViewModel()
            {
                DispositionNo = "DispositionNo",
                Id = 1,
                Reason = "Reason",
                SubmitPosition = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION,
                VerifyDate = DateTimeOffset.UtcNow
            };

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_VM_Verification_Disposition()
        {
            PurchasingDispositionVerificationViewModel vm = new PurchasingDispositionVerificationViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }
        [Fact]
        public void Should_Success_Validate_Wrong_Verify_Date_VM_Verification_Disposition()
        {
            PurchasingDispositionVerificationViewModel vm = new PurchasingDispositionVerificationViewModel()
            {
                DispositionNo = "DispositionNo",
                Id = 1,
                Reason = "Reason",
                SubmitPosition = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION,
                VerifyDate = DateTimeOffset.UtcNow.AddDays(1)
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Get_Report()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientFromPurchasingDisposition());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });

            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var postedModel = _dataUtil(service).GetNewData();
            HttpClientFromPurchasingDisposition httpClientFromPurchasingDisposition = new HttpClientFromPurchasingDisposition();
            postedModel.DispositionNo = httpClientFromPurchasingDisposition.GetPurchasingDispositionViewModel().DispositionNo;
            await service.CreateAsync(postedModel);
            var model = await service.ReadByIdAsync(postedModel.Id);
            var reportResponse = service.GetReport(1, 25, "{}", "{}", null, null, 7);
            Assert.NotEmpty(reportResponse.Data);
            reportResponse = service.GetReport(1, 25, "{}", "{}", DateTimeOffset.UtcNow.AddDays(-1), null, 7);
            Assert.NotEmpty(reportResponse.Data);
            reportResponse = service.GetReport(1, 25, "{}", "{}", null, DateTimeOffset.UtcNow.AddDays(1), 7);
            Assert.NotEmpty(reportResponse.Data);
            reportResponse = service.GetReport(1, 25, "{}", "{}", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1), 7);
            Assert.NotEmpty(reportResponse.Data);
        }

        [Fact]
        public void Should_Fail_Get_Report()
        {
            var service = new PurchasingDispositionExpeditionService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
            Assert.Throws<Exception>(() => service.GetReport(1, 25, "{}", "{}", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1), 7));

        }

        [Fact]
        public async void Should_Success_Generate_Excel()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientFromPurchasingDisposition());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });

            PurchasingDispositionExpeditionService service = new PurchasingDispositionExpeditionService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var postedModel = _dataUtil(service).GetNewData();
            HttpClientFromPurchasingDisposition httpClientFromPurchasingDisposition = new HttpClientFromPurchasingDisposition();
            postedModel.DispositionNo = httpClientFromPurchasingDisposition.GetPurchasingDispositionViewModel().DispositionNo;
            await service.CreateAsync(postedModel);
            var model = await service.ReadByIdAsync(postedModel.Id);
            var reportResponse = service.GenerateExcel(1, 25, "{}", "{}", null, null, 7);
            Assert.NotNull(reportResponse);
        }
    }
}