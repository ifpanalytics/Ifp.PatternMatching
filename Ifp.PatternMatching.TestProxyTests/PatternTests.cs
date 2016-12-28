using Ifp.PatternMatching.TestProxyTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Tests
{
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
    }
}