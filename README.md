# Introduction

Ifp.PatternMatching is a library that brings [functional pattern matching](https://en.wikipedia.org/wiki/Pattern_matching) to C#. 

This library is made by [Bob Nystrom](https://github.com/munificent) and was originally published 2009 in this [article](http://journal.stuffwithstuff.com/2009/05/13/ml-style-pattern-matching-in-c/).
The code was pasted to bitbucket [bitbucket.org/munificent/pattern_matching](https://bitbucket.org/munificent/pattern_matching). 
Based on that work a portable class library was created, tests were added, nuget packages created and finally brought to github.

The library can be used to build business rules that inspect a type hierarchy and apply rules on types that meet some criteria:

```CS
//sub-type matching with conditions on the sub-type
var specialFrontendOffers = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
    Case<AppShoppingCart>(appShopingCart => appShopingCart.IsFirstRunExperience, 0.05m). //5% off for the first time order with the app
    Case<WebShoppingCart>(webShopingCart => webShopingCart.PromoCode == "WebSpecial", 0.04m). //4% off for the newsletter promotion code (only supported by the web interface)
    Default(0.0m).
    Result;

//matching on a condition and nested pattern matching
var cartDiscounts = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
    Case(cart => cart.OrderValue > 100, cart => Pattern.Match<Customer, decimal>(cart.Customer). // if the order value is bigger than 100 the discount depends on the customer status
        Case<ClubMember>(0.1m). // Club member always get 10%
        Case<StandardCustomer>(standardCustomer => !standardCustomer.HasOutstandingDebts, 0.05m). // standardCustomers get 5% if there are no outstanding debts
        Default(0.0m).
        Result).
    Case(cart => cart.OrderValue > 50, 0.02m). // between 50 and 100, the discount is 2% without further conditions
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
``` 

# How to use

The `Pattern.Match` can be used either as expression:

```CS
// map US grades to German grades (schulnote) 
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

If used as an expression the pattern matching looks like this:

```CS
//start the pattern-match by specifying the source and target type and passing an object of the source-type.
var objOfTargetType=Pattern.Match<TSourceType, TTargetType>(objOfSourceType).  
    Case(...).  //specify cases (see below)
    Case(...).
    Default(...). //specify a default value
    Result; //ask for the result. Throws exception if there is no match
```

`Case` consist of three parts `Case<Type parameter>(Predicate, Return value);`

1. *Optional* **Type parameter**. The type of value to match. `TCase` must be a sub-type of the `TSourceType`
2. *Optional* **Predicate** The predicate to evaluate to test the match. The predicate can be either
    * A concrete value or
    * A predicate function of type `Func<TCase, bool>` or `Func<bool>`
3. The **Return value**. Can be
    * Either a concrete value of type `TTargetType` or
    * A function that produces a `TTargetType`. This can either be a `Func<TCase, TTargetType>` or a `Func<TTargetType>`. 

`Result` is optional because the `ReturnMatcher` can implicit be converted to the TargetType:

```CS
int number = Pattern.Match<string, int>("III"). 
    Case("I", 1).
    Case("II", 2).
    Case("III", 3). // 'Case' returns a ReturnMatcher that is implicit converted to an int.
    Case("IV", 4).
    Case("V", 5);
```

### Example 1) Matching on a type and conditions

Match any given `animal` to one special ability by applying this rules:

* If the `animal` is of type `Dog` and is a `search and rescue dog` then the special ability is *scenting*.
* If the `animal` is of type `chicken` and is male then the special ability is *crowing*.
* Otherwise it doesn't have a special ability.     

```CS
var specialAbility = Pattern.Match<Animal, SpecialAbility>(animal).
    Case<Dog>(d => d.IsSearchAndRescueDog, SpecialAbility.Scenting).
    Case<Chicken>(c => c.Gender == Gender.Male, SpecialAbility.Crow).
    Default(SpecialAbility.None).
    Result;
```

By using C#6 features this example can be used like this:

```CS
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
```

### Example 2) Matching and returning by inspecting details of the source object

Calculate the discount of a shopping cart by applying this rules:

* If the customer is `ClubMember` the discount is 5% of the carts order value.
* If the customer is `FirstTimeCustomer` the discount is 4% of the carts order value 
* If the customer is `StandardCustomer` the discount is 2% of the carts order value
* Otherwise there is no discount.

```CS
var discount = Pattern.Match<ShoppingCart, decimal>(shoppingCart).
    Case(cart => cart.Customer is ClubMember, cart => cart.OrderValue * 0.05m).
    Case(cart => cart.Customer is FirstTimeCustomer, cart => cart.OrderValue * 0.04m).
    Case(cart => cart.Customer is StandardCustomer, cart => cart.OrderValue * 0.02m).
    Default(0.0m).
    Result;
```

### Example 3) Incomplete cases throw a `NoMatchException`

In the following example the type `FirstTimeCustomer` isn't in the case list.
Accessing the `Result` property raises a `NoMatchException`.

```CS
var customer = new FirstTimeCustomer();
var discount = Pattern.Match<Customer, decimal>(customer).
    Case<ClubMember>(0.05m).
    Case<StandardCustomer>(0.02m).
    Result;
``` 

## Use as a statement

If used as a statement the pattern matching looks like this:

```CS
//start the pattern-match by passing an object of the source-type.
Pattern.Match(objOfSourceType).  
    Case(...).  //specify cases (see below)
    Case(...).
    Default(...); //specify a default action
```

`Case` consist of three parts `Case<Type parameter>(Predicate, Action);`

1. *Optional* **Type parameter**. The type of value to match. `TCase` must be a sub-type of the `TSourceType`
2. *Optional* **Predicate** The predicate to evaluate to test the match. The predicate can be either
    * A concrete value or
    * A predicate function of type `Func<TCase, bool>` or `Func<bool>`
3. The **Action**. This can either be an `Action<TCase>` or an `Action`. 

### Example 4)

Let the animal make a noise.

```CS
var animal = new Dog(Gender.Male);
Pattern.Match<Animal>(animal).
    Case<Chicken>(c => c.Gender == Gender.Male, c => c.Cockadoodledoo()).
    Case<Dog>(d => d.Bark());
```
The type parameter when calling the `Match` method is usually not needed:

```CS
Animal animal = new Dog(Gender.Male);
Pattern.Match(animal). //Type 'Animal' correctly inferred
    Case<Chicken>(c => c.Gender == Gender.Male, c => c.Cockadoodledoo()).
    Case<Dog>(d => d.Bark());
```

## Special use cases

The library supports the extraction of properties during a match to allow [decomposition](http://hestia.typepad.com/flatlander/2010/07/f-pattern-matching-for-beginners-part-2-decomposition.html):

```CS
Pattern.Match(animal).
    Case<Dog, Furs>(fur => WashMe(fur)). // Dog has a property of type Furs that is extracted from the dog instance.
    Case<Chicken, Featherings>(feathering => MakeUnableToFly(feathering));
```

To support decomposition the source object needs to implement the `IMatchable` interface:

```CS
public class Dog : Animal, IMatchable<Furs> //Implement IMatchable 
{
    public Furs Fur { get; } // The property that is enabled for decomposition in pattern matching.
    
    Furs IMatchable<Furs>.GetArg() => this.Fur; //Explicit interface implementation.
}
```

It is possible to support up to four decomposable object properties. 

# How to get

The library can be installed via nuget:
[https://www.nuget.org/packages/Ifp.PatternMatching/](https://www.nuget.org/packages/Ifp.PatternMatching/)

Install via [Package Manager Console](https://docs.nuget.org/ndocs/tools/package-manager-console)
```CS
PS> Install-Package Ifp.PatternMatching
```