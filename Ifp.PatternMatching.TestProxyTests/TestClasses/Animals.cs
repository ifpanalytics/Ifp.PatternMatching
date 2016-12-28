using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.PatternMatching.TestProxyTests.TestClasses
{
    public enum AnimalTypes
    {
        Dog,
        Chicken,
    }

    public enum Gender
    {
        Male,
        Female,
    }
    public abstract class Animal
    {
        public Animal(Gender gender)
        {
            Gender = gender;
        }
        public Gender Gender { get; }
    }

    public class Dog : Animal
    {
        public Dog(Gender gender) : base(gender)
        {

        }
    }
    public class Chicken : Animal
    {
        public Chicken(Gender gender) : base(gender)
        {

        }
    }
}
