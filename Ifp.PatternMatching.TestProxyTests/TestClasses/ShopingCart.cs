using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.PatternMatching.TestProxyTests.TestClasses
{
    public abstract class Customer
    {
    }

    public class FirstTimeCustomer : Customer
    {
    }

    public class StandardCustomer : Customer
    {
        public bool HasOutstandingDebts { get; }
    }

    public class ClubMember : StandardCustomer
    {
        public string MembershipNumber { get; }
        public int MembershipAge { get; }
    }


    public abstract class PayMethod
    {

    }
    public class CreditCard: PayMethod
    {

    }
    public class AdvancePayment : PayMethod
    {

    }

    public class Address
    {
        public float ShippingDistance { get; }
    }

    public abstract class ShoppingCart
    {
        public ShoppingCart()
        {
            Customer = new StandardCustomer();
            ShippingAddress = new Address();
            PayMethod = new AdvancePayment();
        }

        public int OrderValue { get; }
        public Customer Customer { get; }
        public Address ShippingAddress { get; }
        public PayMethod PayMethod { get; }
    }

    public class WebShoppingCart : ShoppingCart
    {
        public string PromoCode { get; }
    }
    public class AppShoppingCart: ShoppingCart
    {
        public bool IsFirstRunExperience { get; }
    }
}
