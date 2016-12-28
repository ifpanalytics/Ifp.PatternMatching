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
            var dog = new Dog(Gender.Male);
            var actionWasCalled = false;
            Pattern.Match(dog).Case(dog, () => actionWasCalled = true);
            Assert.IsTrue(actionWasCalled, "match action must be called.");
        }

        [TestMethod()]
        public void OnlyOneMatchIsExecuted()
        {
            var dog = new Dog(Gender.Male);
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
            var dog = new Dog(Gender.Male);
            var chicken = new Chicken(Gender.Male);
            var actionWasCalled = false;
            Pattern.Match<Animal>(dog).
                Case(chicken, () => actionWasCalled = true);
            Assert.IsFalse(actionWasCalled, "match action must not be called.");
        }

        [TestMethod()]
        public void ObjectMatchesItselfAndIsPassedToTheActionDelegate()
        {
            var dog = new Dog(Gender.Male);
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
            var dog = new Dog(Gender.Male);
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
            var dog = new Dog(Gender.Male);
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
        public void TrueReturningPredicateActionIsCalled()
        {
            var dog = new Dog(Gender.Male);
            var action1WasCalled = false;
            var action2WasCalled = false;
            Pattern.Match(dog).
                Case(d => { Assert.AreSame(dog, d, "Passed Object must be dog."); return false; }, () => action1WasCalled = true).
                Case(d => { Assert.AreSame(dog, d, "Passed Object must be dog."); return true; }, () => action2WasCalled = true);
            Assert.IsFalse(action1WasCalled, "match action1 must not be called");
            Assert.IsTrue(action2WasCalled, "match action2 must be called");
        }

        [TestMethod()]
        [ExpectedException(typeof(NoMatchException), "No match must raise a NoMatchException.")]
        public void NoMatchRaisesExceptionOnFunctions()
        {
            var dog = new Dog(Gender.Male);
            var chicken = new Chicken(Gender.Male);
            var test = Pattern.Match<Animal, bool>(dog).
                Case(chicken, () => true).
                Result;
        }
    }
}