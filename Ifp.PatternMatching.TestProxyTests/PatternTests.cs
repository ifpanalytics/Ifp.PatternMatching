﻿using Ifp.PatternMatching.TestProxyTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Tests
{
    public enum ReturnMatcherResult
    {
        Result1,
        Result2,
        Result3,
        Result4
    }

    [TestClass()]
    public class PatternTests
    {

        [TestMethod()]
        public void ObjectMatchesItself()
        {
            Animal dog = new Dog(Gender.Male);
            var actionWasCalled = false;
            Pattern.Match(dog).Case(dog, () => actionWasCalled = true);
            Assert.IsTrue(actionWasCalled, "match action must be called.");
        }

        [TestMethod()]
        public void OnlyOneMatchIsExecuted()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case(dog, () => action1WasCalled = true).
                Case(dog, () => action2WasCalled = true);
            Assert.IsTrue(action1WasCalled, "match action1 must be called.");
            Assert.IsFalse(action2WasCalled, "match action2 must not be called.");
        }

        [TestMethod()]
        public void NoMatchPassesOnActions()
        {
            Animal dog = new Dog(Gender.Male);
            Chicken chicken = new Chicken(Gender.Male);
            var actionWasCalled = false;
            Pattern.Match(dog).
                Case(chicken, () => actionWasCalled = true);
            Assert.IsFalse(actionWasCalled, "match action must not be called.");
        }

        [TestMethod()]
        public void ObjectMatchesItselfAndIsPassedToTheActionDelegate()
        {
            Animal dog = new Dog(Gender.Male);
            var actionWasCalled = false;
            Pattern.Match(dog).Case(dog, d =>
            {
                Assert.AreSame(d, dog, "passed object is not the same.");
                actionWasCalled = true;
            });
            Assert.IsTrue(actionWasCalled, "match action must be called");
        }

        [TestMethod()]
        public void BoolReturningCaseFunctionMatchesOnTrue()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case(() => false, () => action1WasCalled = true).
                Case(() => true, () => action2WasCalled = true);
            Assert.IsFalse(action1WasCalled, "match action must be called");
            Assert.IsTrue(action2WasCalled, "match action must be called");
        }

        [TestMethod()]
        public void BoolReturningCaseFunctionMatchesOnTrueAndPassesTestValue()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case(() => false, () => action1WasCalled = true).
                Case(() => true, d =>
                {
                    Assert.AreSame(dog, d, "Passed Object must be dog.");
                    action2WasCalled = true;
                });
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        public void TrueReturningPredicateActionIsCalledWithTheRightParameter()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case(d => { Assert.AreSame(dog, d, "Passed Object must be dog."); return false; }, d => action1WasCalled = true).
                Case(d => { Assert.AreSame(dog, d, "Passed Object must be dog."); return true; }, d => { Assert.AreSame(dog, d, "Passed Object must be dog."); action2WasCalled = true; });
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        public void TypeMatchingActionIsCalled()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case<Chicken>(() => action1WasCalled = true).
                Case<Dog>(() => action2WasCalled = true);
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        public void TypeMatchingActionIsCalledWithCastParameter()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case<Chicken>(c => { c.Cockadoodledoo(); action1WasCalled = true; }).
                Case<Dog>(d => { d.Bark(); action2WasCalled = true; });
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        public void TypeMatchingWithPredicateActionIsCalledWithCastParameter()
        {
            Animal dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            var action3WasCalled = false;
            var action4WasCalled = false;
            Pattern.Match(dog).
                Case<Chicken>(c => c.Gender == Gender.Female, c => { action1WasCalled = true; }).
                Case<Chicken>(c => c.Gender == Gender.Male, c => { c.Cockadoodledoo(); action2WasCalled = true; }).
                Case<Dog>(d => d.Gender == Gender.Female, d => { d.Bark(); action3WasCalled = true; }).
                Case<Dog>(d => d.Gender == Gender.Male, d => { d.Bark(); action4WasCalled = true; });
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsFalse(action2WasCalled, "match action2 must not be called");
            Assert.IsFalse(action3WasCalled, "match action3 must not be called");
            Assert.IsTrue(action4WasCalled, "match action4 must be called");
        }

        [TestMethod()]
        public void OneFieldExtractedByMatch()
        {
            Animal dog = new Dog(Gender.Male, Furs.Blond);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case<Dog, Featherings>(f => action1WasCalled = true).
                Case<Dog, Furs>(f => { Assert.AreEqual(Furs.Blond, f); action2WasCalled = true; });
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        [ExpectedException(typeof(NoMatchException), "No match must raise a NoMatchException.")]
        public void NoMatchRaisesExceptionOnFunctions()
        {
            Animal dog = new Dog(Gender.Male);
            Animal chicken = new Chicken(Gender.Male);
            var test = Pattern.Match<Animal, bool>(dog).
                Case(chicken, () => true).
                Result;
        }

        [TestMethod()]
        public void ReturnMatcherMatchesOnObject()
        {
            Animal dog = new Dog(Gender.Male);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(dog).
                Case(dog, ReturnMatcherResult.Result1);
            Assert.AreEqual(ReturnMatcherResult.Result1, result);
        }

        [TestMethod()]
        public void ReturnMatcherMatchesOnObjectAndReturnsFromDelegate()
        {
            Animal dog = new Dog(Gender.Male);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(dog).
                Case(dog, () => ReturnMatcherResult.Result1);
            Assert.AreEqual(ReturnMatcherResult.Result1, result);
        }

        [TestMethod()]
        public void ReturnMatcherMatchesOnObjectAndObjectIsPassedToDelegate()
        {
            Animal dog = new Dog(Gender.Male);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(dog).
                Case(dog, d => { Assert.AreSame(dog, d); return ReturnMatcherResult.Result1; });
            Assert.AreEqual(ReturnMatcherResult.Result1, result);
        }

        [TestMethod()]
        public void ReturnMatcherMatchesOnType()
        {
            Animal dog = new Dog(Gender.Male);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(dog).
                Case<Chicken>(ReturnMatcherResult.Result2).
                Case<Dog>(ReturnMatcherResult.Result1);
            Assert.AreEqual(ReturnMatcherResult.Result1, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnTypeAndPredicateReturnsByValue()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case<Dog>(d => ReturnMatcherResult.Result1).
                Case<Chicken>(c => c.Feathering == Featherings.Flecked, ReturnMatcherResult.Result2).
                Case<Chicken>(c => c.Feathering == Featherings.Black, ReturnMatcherResult.Result3).
                Case<Chicken>(ReturnMatcherResult.Result4).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result3, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnTypeAndPredicateReturnsByFunc1()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case<Dog>(d => ReturnMatcherResult.Result1).
                Case<Chicken>(c => c.Feathering == Featherings.Flecked, () => ReturnMatcherResult.Result2).
                Case<Chicken>(c => c.Feathering == Featherings.Black, () => ReturnMatcherResult.Result3).
                Case<Chicken>(() => ReturnMatcherResult.Result4).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result3, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnTypeAndPredicateReturnsByFunc2()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case<Dog>(d => ReturnMatcherResult.Result1).
                Case<Chicken>(c => c.Feathering == Featherings.Flecked, c => { Assert.AreEqual(animal, c); return ReturnMatcherResult.Result2; }).
                Case<Chicken>(c => c.Feathering == Featherings.Black, c => { Assert.AreEqual(animal, c); return ReturnMatcherResult.Result3; }).
                Case<Chicken>(c => { Assert.AreEqual(animal, c); return ReturnMatcherResult.Result4; }).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result3, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnPredicateAndReturnsValue()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case(a => a.Gender == Gender.Female, ReturnMatcherResult.Result1).
                Case(a => a.Gender == Gender.Male, ReturnMatcherResult.Result2).
                Default(ReturnMatcherResult.Result3).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result2, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnPredicateAndReturnsValueFromFunc1()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case(a => a.Gender == Gender.Female, () => ReturnMatcherResult.Result1).
                Case(a => a.Gender == Gender.Male, () => ReturnMatcherResult.Result2).
                Default(ReturnMatcherResult.Result3).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result2, result);
        }

        [TestMethod]
        public void ReturnMatcherMatchesOnPredicateAndReturnsValueFromFunc2()
        {
            Animal animal = new Chicken(Gender.Male, Featherings.Black);
            var result = Pattern.Match<Animal, ReturnMatcherResult>(animal).
                Case(a => a.Gender == Gender.Female, a => { Assert.AreSame(animal, a); return ReturnMatcherResult.Result1; }).
                Case(a => a.Gender == Gender.Male, a => { Assert.AreSame(animal, a); return ReturnMatcherResult.Result2; }).
                Default(ReturnMatcherResult.Result3).
                Result;
            Assert.AreEqual(ReturnMatcherResult.Result2, result);
        }
    }
}