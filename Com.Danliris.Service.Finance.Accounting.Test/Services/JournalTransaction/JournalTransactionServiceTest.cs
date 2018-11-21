﻿using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.JournalTransaction
{
    public class JournalTransactionServiceTest
    {
        private const string ENTITY = "JournalTransaction";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

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

        private JournalTransactionDataUtil _dataUtil(JournalTransactionService service)
        {
            var coaService = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var coaDataUtil = new COADataUtil(coaService);
            return new JournalTransactionDataUtil(service, coaDataUtil);
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

        [Fact]
        public async void Should_Success_Get_Data()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var Response = service.Read(1, 25, "{}", null, data.DocumentNo, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate();

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            var vm = new JournalTransactionViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Update_Data()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);
            newModel.Description = "NewDescription";
            var Response = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            //var modelToDelete = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(model.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Error_Validate_Data_NotEqual_Total()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate();
            vm.Items[1].Credit = 5000;
            vm.Date = DateTimeOffset.UtcNow.AddDays(2);

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Error_Validate_Data_Debit_And_Credit_Exist()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate();
            vm.Items[1].Debit = 5000;
            vm.Items[1].Credit = 1000;
            vm.Items[1].COA = null;

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Error_Validate_Data_Debit_And_Credit_EqualsZero()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate();
            vm.Items[1].Credit = 0;
            vm.Items[1].Credit = 0;

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Update_Data_NewItem()
        {
            var service = new JournalTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData();
            model.Description = "NewDescription";

            var newItemDebit = new JournalTransactionItemModel()
            {
                COA = new COAModel()
                {
                    Id = 1,
                    Code = ""
                },
                Debit = 100
            };
            model.Items.Add(newItemDebit);

            var newItemCredit = new JournalTransactionItemModel()
            {
                COA = new COAModel()
                {
                    Id = 1,
                    Code = ""
                },
                Credit = 100
            };
            model.Items.Add(newItemCredit);

            var Response = await service.UpdateAsync(model.Id, model);
            Assert.NotEqual(0, Response);
        }
    }
}