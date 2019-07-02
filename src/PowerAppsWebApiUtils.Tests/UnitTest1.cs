using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerAppsWebApiUtils.Configuration;
using PowerAppsWebApiUtils.Json;
using PowerAppsWebApiUtils.Repositories;
using PowerAppsWebApiUtils.Security;
using webapi.entities;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetOneTest()
        {
            var config = PowerAppsConfigurationReader.GetConfiguration();
            var tokenProvider = new AuthenticationMessageHandler(config);

            var repo = new GenericRepository<Account>(tokenProvider);
            var entityId = Guid.Parse("BB7F2EEC-A38C-E911-A985-000D3AF49637");
            var account = await repo.GetById(entityId, //null
            new Expression<Func<Account, object>>[]
            {
                p => p.Id, 
                p => p.StateCode, 
                p => p.StatusCode,
                p => p.LastOnHoldTime,
                p => p.ModifiedOn,
                p => p.CreatedOn,
                p => p.CreatedBy,
                p => p.OwnerId,
                p => p.ParentAccountId,
                p => p.Telephone1,
                p => p.Name,
            }
            );
            Assert.IsNotNull(account);
            Assert.IsNotNull(account.Name);
            Assert.IsNotNull(account.CreatedBy);
            Assert.IsNotNull(account.OwnerId);
            Assert.AreEqual(entityId, account.Id);
            Assert.AreEqual<account_statecode?>(account_statecode.Active, account.StateCode);            
            Assert.AreEqual<account_statuscode?>(account_statuscode.Active, account.StatusCode);            
            Assert.IsNull(account.LastOnHoldTime);
            Assert.IsNotNull(account.ModifiedOn);
            Assert.IsNotNull(account.CreatedOn);

            var json = JObject.FromObject(account, new JsonSerializer{ ContractResolver = new NavigationPropertyContractResolver() });
        }

        [TestMethod]
        public async Task GetMultipleTest()
        {
            var config = PowerAppsConfigurationReader.GetConfiguration();

            using (var tokenProvider = new AuthenticationMessageHandler(config))
            using(var repo = new GenericRepository<Account>(tokenProvider))
            {
                var accounts = await repo.GetList();
                Assert.IsNotNull(accounts);
            }
        }

        
        [TestMethod]
        public async Task CreateAccountTest()
        {
            var config = PowerAppsConfigurationReader.GetConfiguration();

            using (var tokenProvider = new AuthenticationMessageHandler(config))
            using(var repo = new GenericRepository<Account>(tokenProvider))
            {
                // var account = new Account 
                // {
                //     Name = Guid.NewGuid().ToString(),
                //     AccountCategoryCode = account_accountcategorycode.Standard,
                //     AccountClassificationCode = account_accountclassificationcode.DefaultValue,
                //     AccountRatingCode = account_accountratingcode.DefaultValue,
                //     AccountNumber = "11111111",
                //     Address1_AddressTypeCode = account_address1_addresstypecode.Primary,
                //     Address1_City = "Montreal",
                //     Address1_Country = "Canada",
                //     Address1_PostalCode = "H1H 1H1",
                //     Address1_StateOrProvince = "QC",
                //     DoNotEMail = true,
                //     DoNotPhone = false,
                //     CreditLimit = 500000.99m,
                //     EMailAddress1 = string.Empty,
                //     Telephone1 = "Telephone1",
                //     Fax = "Fax",
                //     WebSiteURL = "WebSiteURL",
                //     LastOnHoldTime = new DateTime(2019, 1, 1, 0, 0, 0)
                // };  

                // var accountid = await repo.Create(account);

                //Assert.IsNotNull(accountid);

                var account = 
                    await repo.GetById(
                        Guid.Parse("72e4bfa0-836a-e911-a98a-000d3af49373"), 
                        new Expression<Func<Account, object>>[]
                        {
                            p => p.Id, 
                            p => p.StateCode, 
                            p => p.StatusCode,
                            p => p.LastOnHoldTime,
                            p => p.ModifiedOn,
                            p => p.CreatedOn,
                            p => p.CreatedBy,
                            p => p.OwnerId,
                            p => p.ParentAccountId,
                            p => p.Telephone1,
                        });

                var owner = account.OwnerId;
            }

        }

         [TestMethod]
        public async Task UpdateAdressParentAccountTest()
        {
            var config = PowerAppsConfigurationReader.GetConfiguration();

            using (var tokenProvider = new AuthenticationMessageHandler(config))
            using(var repo = new GenericRepository<CustomerAddress>(tokenProvider))
            {
                var address = 
                    new CustomerAddress(Guid.Parse("83ca70b4-0d9a-e911-a98c-000d3af49373"))
                    {
                        City = "Montreal",
                        AddressTypeCode = customeraddress_addresstypecode.BillTo,
                        ParentId = new Account(Guid.Parse("72e4bfa0-836a-e911-a98a-000d3af49373")).ToNavigationProperty(),
                    };

                 await repo.Update(address);
            }
        }
    
        [TestMethod]
        public async Task GetcustomerAddressesTest()
        {
            var config = PowerAppsConfigurationReader.GetConfiguration();

            using (var tokenProvider = new AuthenticationMessageHandler(config))
            using(var repo = new GenericRepository<Account>(tokenProvider))
            {
                var addresses = await repo.GetList();
            }
        }

    }
}