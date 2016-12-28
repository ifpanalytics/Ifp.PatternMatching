# Introduction

Ifp.PatternMatching is a library that brings [functional pattern matching](https://en.wikipedia.org/wiki/Pattern_matching) to C#. 

This library is made by [Bob Nystrom](https://github.com/munificent) and was originally published 2009 in this [article](http://journal.stuffwithstuff.com/2009/05/13/ml-style-pattern-matching-in-c/).
The code was pasted to bitbucket [bitbucket.org/munificent/pattern_matching](https://bitbucket.org/munificent/pattern_matching). 
Based on that work a portable class library was created, tests were added, nuget packages created and finally brought to github.

The library can be used to build business rules that inspect a type hierarchy and apply rules on types that meet some criteria:

```CS
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
    Case(cart => cart.OrderValue > 50, 0.02m). // between 50 and 100, the discount is 2% without furher conditions
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
``` 

# How to use

The `Pattern.Match` can be used either as expression:

```CS
// map US grades to german grades (schulnote) 
var grade = "A";
var schulnote = Pattern.Match<string, int>(grade).
    Case("A", 1).
    Case("B", 2).
    Case("C", 3).
    Case("D", 4).
    Case("E", 5).
    Result;
```  

or as a statement:

```CS
// let an animal make a noise 
Animal animal = new Chicken(Gender.Male);
Pattern.Match(animal).
    Case<Dog>(d => d.Bark()).
    Case<Chicken>(c => c.Gender == Gender.Male, c => c.Cockadoodledoo());
```  

## Use as an expression

Match any given `animal` to one special ability by applying this rules:
* If the `animal` is of type `Dog` and is a `search and rescue dog` then the special ability is *scenting*.
* If the `animal` is of type `chicken` and is male then the special ability is `crowing`.
* Otherwise it doesn't have a special ability.     

```CS
var specialAbility = Pattern.Match<Animal, SpecialAbility>(animal).
    Case<Dog>(d => d.IsSearchAndRescueDog, SpecialAbility.Scenting).
    Case<Chicken>(c => c.Gender == Gender.Male, SpecialAbility.Crow).
    Default(SpecialAbility.None);
```

TODO: Further examples

## Use as a statement

TODO: Further examples

## Special use cases

TODO: IMatchable<T1,...> and field extraction overloads.

# How to get

TODO: Publish on nuget.org