using Ifp.PatternMatching.TestProxyTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static PatternMatching.Pattern;
namespace AnimalRules
{
    public class AnimalFacts
    {
        public SpecialAbility GetSpecialAbilityOf(Animal animal) =>
            Match<Animal, SpecialAbility>(animal).
            Case<Dog>(d => d.IsSearchAndRescueDog, SpecialAbility.Scenting).
            Case<Chicken>(c => c.Gender == Gender.Male, SpecialAbility.Crow).
            Default(SpecialAbility.None).
            Result;
    }
}

namespace Ifp.PatternMatching.TestProxyTests
{
    [TestClass()]
    public class DocumentationTests
    {
        [TestMethod()]
        public void ShopingCardTest()
        {
            ShoppingCart shoppingCart = new WebShoppingCart();

            //sub-type matching with conditions on the sub-type
            var specialFrontendOffers = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
                Case<AppShoppingCart>(appShopingCart => appShopingCart.IsFirstRunExperience, 0.05m). //5% off for the first time order with the app
                Case<WebShoppingCart>(webShopingCart => webShopingCart.PromoCode == "WebSpecial", 0.04m). //4% off for the newsletter promotion code (only supported by the web interface)
                Default(0.0m).
                Result;

            //matching on a condition and nested pattern matching
            var cartDiscounts = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
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

            var overallPrice = shoppingCart.OrderValue +
                (shoppingCart.OrderValue * specialFrontendOffers) +
                (shoppingCart.OrderValue * cartDiscounts) +
                shipping;

            //Start check out process
            Pattern.Match(shoppingCart.PayMethod).
                Case<CreditCard>(creditCard => CheckoutPerCreditcard(shoppingCart, overallPrice)).
                Case<AdvancePayment>(advancePayment => CheckoutPerAdvancePayment(shoppingCart, overallPrice)).
                Default(() => { throw new NotSupportedException(); });
        }

        private void CheckoutPerAdvancePayment(ShoppingCart shoppingCart, decimal overallPrice)
        {
        }

        private void CheckoutPerCreditcard(ShoppingCart shoppingCart, decimal overallPrice)
        {
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
            Assert.AreEqual(1, schulnote);
        }

        [TestMethod()]
        public void TypeCheckIfMaleChicken()
        {
            Animal animal = new Chicken(Gender.Male);
            var specialAbility = Pattern.Match<Animal, SpecialAbility>(animal).
                Case<Dog>(d => d.IsSearchAndRescueDog, SpecialAbility.Scenting).
                Case<Chicken>(c => c.Gender == Gender.Male, SpecialAbility.Crow).
                Default(SpecialAbility.None);
            Assert.AreEqual(SpecialAbility.Crow, specialAbility);
        }

        public SpecialAbility GetSpecialAbilityOf(Animal animal) =>
            Match<Animal, SpecialAbility>(animal).
                Case<Dog>(d => d.IsSearchAndRescueDog, SpecialAbility.Scenting).
                Case<Chicken>(c => c.Gender == Gender.Male, SpecialAbility.Crow).
                Default(SpecialAbility.None);

        [TestMethod]
        public void ImplicitConversion()
        {
            int number = Pattern.Match<string, int>("III").
                Case("I", 1).
                Case("II", 2).
                Case("III", 3). // 'Case' returns a RetrunMatcher that is implicit converted to an int.
                Case("IV", 4).
                Case("V", 5);
            Assert.AreEqual(3, number);
        }

        [TestMethod]
        public void InspectTheObjectAndCalcualteAResult()
        {
            var shoppingCart = new WebShoppingCart();
            var discount = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
                Case(cart => cart.Customer is ClubMember, cart => cart.OrderValue * 0.05m).
                Case(cart => cart.Customer is FirstTimeCustomer, cart => cart.OrderValue * 0.04m).
                Case(cart => cart.Customer is StandardCustomer, cart => cart.OrderValue * 0.02m).
                Default(cart => cart.OrderValue).
                Result;
        }

        [TestMethod]
        [ExpectedException(typeof(NoMatchException))]
        public void IncompleteCases()
        {
            var customer = new FirstTimeCustomer();
            var discount = Pattern.Match<Customer, decimal>(customer).
                Case<ClubMember>(0.05m).
                Case<StandardCustomer>(0.02m).
                Result;
        }

        [TestMethod]
        public void StatementMakeNoise()
        {
            var animal = new Dog(Gender.Male);
            Pattern.Match<Animal>(animal).
                Case<Chicken>(c=>c.Gender==Gender.Male, c => c.Cockadoodledoo()).
                Case<Dog>(d => d.Bark());
        }
    }
}

