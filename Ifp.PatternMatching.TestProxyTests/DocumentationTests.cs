using Ifp.PatternMatching.TestProxyTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.PatternMatching.TestProxyTests
{
    [TestClass()]
    public class DocumentationTests
    {
        [TestMethod()]
        public void ShopingCardTest()
        {
            ShopingCart shoppingCart = new WebShopingCart();

            //sub-type matching with conditions on the sub-type
            var specialFrontendOffers = Pattern.Match<ShopingCart, decimal>(shoppingCart).
                Case<AppShopingCart>(appShopingCart => appShopingCart.IsFirstRunExperience, 0.05m). //5% off for the first time order with the app
                Case<WebShopingCart>(webShopingCart => webShopingCart.PromoCode == "WebSpecial", 0.04m). //4% off for the newsletter promotion code (only supported by the web interface)
                Default(0.0m).
                Result;

            //matching on a condition and nested pattern matching
            var cartDiscounts = Pattern.Match<ShopingCart, decimal>(shoppingCart).
                Case(cart => cart.OrderValue > 100, cart => Pattern.Match<Customer, decimal>(cart.Customer). // if the order value is bigger than 100 the discount depends on the customer status
                    Case<ClubMember>(0.1m). // Clubmember always get 10%
                    Case<StandardCustomer>(standardCustomer => !standardCustomer.HasOutstandingDebts, 0.05m). // standardCustomers get 5% if there are no outstanding debts
                    Default(0.0m).
                    Result).
                Case(cart => cart.OrderValue > 50, 0.02m). // between 50 and 100, the dscount is 2% without furher conditions
                Default(0.0m).
                Result;

            //after the first match all the other matches are ignored
            var shipping = Pattern.Match<Address, decimal>(shoppingCart.ShippingAddress).
                Case(address => address.ShippingDistance > 1000, 7m).
                Case(address => address.ShippingDistance > 500, 5m).
                Case(address => address.ShippingDistance > 50, 3m).
                Default(2m);

            var overall = shoppingCart.OrderValue +
                (shoppingCart.OrderValue * specialFrontendOffers) +
                (shoppingCart.OrderValue * cartDiscounts) +
                shipping;
        }

        [TestMethod]
        public void SchulNote()
        {
            var grade = "A";
            var schulnote = Pattern.Match<string, int>(grade).
                Case("A", 1).
                Case("B", 2).
                Case("C", 3).
                Case("D", 4).
                Case("E", 5).
                Result;
        }

        [TestMethod]
        public void AnimalNoises()
        {
            Animal animal = new Chicken(Gender.Male);
            Pattern.Match(animal).
                Case<Dog>(d => d.Bark()).
                Case<Chicken>(c => c.Gender == Gender.Male, c => c.Cockadoodledoo());
        }
    }
}