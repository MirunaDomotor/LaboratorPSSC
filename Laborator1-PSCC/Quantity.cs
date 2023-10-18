using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator1PSCC.Quantity;

namespace Laborator1PSCC
{
    [AsChoice]
    public static partial class Quantity
    {
        public interface IQuantity{}

        public record Unit(int Units) : IQuantity 
        {
            public override string ToString()
            {
                return $"{Units} Units";
            }
        }
        public record Kg(double Kgs) : IQuantity
        {
            public override string ToString()
            {
                return $"{Kgs} Kgs";
            }
        }
        public record Undefined(string Undef) : IQuantity
        {
            public override string ToString()
            {
                return $"{Undef} Undefined";
            }
        }
    }
}