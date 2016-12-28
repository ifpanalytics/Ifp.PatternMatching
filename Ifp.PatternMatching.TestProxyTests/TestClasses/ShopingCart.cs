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

    public class Address
    {
        public float ShippingDistance { get; }
    }

    public abstract class ShopingCart
    {
        public ShopingCart()
        {
            Customer = new StandardCustomer();
            ShippingAddress = new Address();
        }

        public int OrderValue { get; }
        public Customer Customer { get; }
        public Address ShippingAddress { get; }
    }

    public class WebShopingCart : ShopingCart
    {
        public string PromoCode { get; }
    }
    public class AppShopingCart: ShopingCart
    {
        public bool IsFirstRunExperience { get; }
    }
}
