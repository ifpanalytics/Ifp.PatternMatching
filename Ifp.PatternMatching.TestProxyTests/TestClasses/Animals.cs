using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.PatternMatching.TestProxyTests.TestClasses
{
    public enum Furs
    {
        Unspecified,
        Black,
        Brown,
        Blond,
        Spotted,
    }

    public enum Featherings
    {
        Unspecified,
        White,
        Black,
        Blue,
        Yellow,
        Red,
        Flecked,
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

    public class Dog : Animal, IMatchable<Furs>
    {
        public Dog(Gender gender) : this(gender, Furs.Unspecified)
        {

        }

        public Dog(Gender gender, Furs fur) : base(gender)
        {
            Fur = fur;
        }

        public Furs Fur { get; }
        public void Bark() { }

        Furs IMatchable<Furs>.GetArg()
        {
            return this.Fur;
        }
    }
    public class Chicken : Animal, IMatchable<Featherings>
    {
        public Chicken(Gender gender) : this(gender, Featherings.Unspecified)
        {

        }
        public Chicken(Gender gender, Featherings feathering) : base(gender)
        {
            Feathering = feathering;
        }

        public Featherings Feathering { get; }

        public void Cockadoodledoo() { }

        Featherings IMatchable<Featherings>.GetArg()
        {
            return Feathering;
        }
    }
}
