using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerAppsWebApiUtils.Linq;
using webapi.entities;

namespace PowerAppsWebApiUtils.Tests
{
    namespace PowerAppsWebApiUtils.Tests
    {
        [TestClass]
        public class LinqProviderQueryTextTests
        {
            [TestMethod]
            public void WhereTest1()
            {
                var context = new WebApiContext(null);
                var query = new Query<Account>(context);
                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual("accounts", command);               
            }

            [TestMethod]
            public void WhereTest2()
            {
                var context = new WebApiContext(null);
                var query = new Query<Account>(context).Where(p => p.Name == "Test");
                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual("accounts?$filter=(name eq 'Test')", command);
            }

            [TestMethod]
            public void WhereTest3()
            {
                var context = new WebApiContext(null);
                var query = new Query<Account>(context).Where(p => p.StateCode == account_statecode.Active);
                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual("accounts?$filter=(statecode eq 0)", command);
            }
           
            [TestMethod]
            public void WhereTest4()
            {
                var context = new WebApiContext(null);
                var query = 
                    new Query<Account>(context)
                    .Where(p => p.Name == "Test")
                    .Where(p => p.StateCode == account_statecode.Active);

                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual("accounts?$filter=(name eq 'Test') and (statecode eq 0)", command);
            }

                       
            [TestMethod]
            public void WhereTest5()
            {
                var context = new WebApiContext(null);
                var query =
                    new Query<Account>(context)
                    .Where(p => p.Name == "Toto" || p.Name == "Tata")
                    .Where(p => p.StateCode == account_statecode.Active);

                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual("accounts?$filter=(name eq 'Toto' or name eq 'Tata') and (statecode eq 0)", command);
            }


            [TestMethod]
            public void WhereTest6()
            {
                var context = new WebApiContext(null);
                var guid = Guid.NewGuid();
                var query =
                    new Query<CustomerAddress>(context)
                    .Where(p => p.ParentId == new Account(guid).ToNavigationProperty())
                    .Where(p => p.ShippingMethodCode == customeraddress_shippingmethodcode.Airborne && p.Country == "Canada");

                var command = context.GetQueryText(query.Expression);

                Assert.AreEqual($"customeraddresses?$filter=(_parentid_value eq '{guid}') and (shippingmethodcode eq 1 and country eq 'Canada')", command);
            }

            [TestMethod]
            public void SelectTest1()
            {
                var context = new WebApiContext(null);
                var guid = Guid.NewGuid();
                var query =
                    new Query<CustomerAddress>(context)
                    .Where(p => p.ParentId == new Account(guid).ToNavigationProperty())
                    .Select(p => new { Id = p.Id, OwnerId = p.OwnerId });

                var command = context.GetQueryText(query.Expression);
                Assert.AreEqual($"customeraddresses?$select=customeraddressid,_ownerid_value&$filter=(_parentid_value eq '{guid}')", command);
            }
        }
    }
}