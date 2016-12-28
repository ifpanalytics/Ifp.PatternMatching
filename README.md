# Introduction

A library that brings [functional pattern matching](https://en.wikipedia.org/wiki/Pattern_matching) to C#. 

This library is made by [Bob Nystrom](https://github.com/munificent) and was originaly published 2009 in this [article](http://journal.stuffwithstuff.com/2009/05/13/ml-style-pattern-matching-in-c/).
The code was pasted to bitbucket [bitbucket.org/munificent/pattern_matching](https://bitbucket.org/munificent/pattern_matching). The library was brought to github, tests were added and nuget packages created.

# How to use

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